using System.Text;
using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Transaction.Service.Clients;
using Play.Transaction.Service.Dtos;
using Play.Transaction.Service.Entities;
using Polly.CircuitBreaker;
using RabbitMQ.Client;

namespace Play.Transaction.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleItemsController : ControllerBase
    {
        private readonly IRepository<SaleItems> saleItemsRepository;
        private readonly IRepository<Sales> salesRepository;
        private readonly ProductClient productClient;

        public SaleItemsController(
            IRepository<SaleItems> saleItemsRepository,
            IRepository<Sales> salesRepository,
            ProductClient productClient
        )
        {
            this.saleItemsRepository = saleItemsRepository;
            this.salesRepository = salesRepository;
            this.productClient = productClient;
        }

        [HttpGet]
        public async Task<IEnumerable<SaleItemsProductDto>> GetAllAsync()
        {
            var saleItems = await saleItemsRepository.GetAllAsync();
            var productIds = saleItems.Select(i => i.ProductId).Distinct().ToList();

            var products = await productClient.GetProductsByIdsAsync(productIds);

            return saleItems.Select(item =>
                item.AsProductDto(
                    products.FirstOrDefault(p => p.Id == item.ProductId)?.ProductName ?? "Unknown"
                )
            );
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleItemsProductDto>> GetByIdAsync(Guid id)
        {
            var item = await saleItemsRepository.GetByIdAsync(id);
            if (item is null)
            {
                return NotFound();
            }

            var product = await productClient.GetProductByIdAsync(item.ProductId);
            if (product is null)
            {
                return NotFound();
            }

            return item.AsProductDto(product.ProductName);
        }

        [HttpPost]
        public async Task<ActionResult<SaleItemsDto>> Post(CreateSaleItemsDto createItemDto)
        {
            // 1. Buat SaleItem baru
            var item = new SaleItems
            {
                Id = Guid.NewGuid(),
                ProductId = createItemDto.ProductId,
                SaleId = createItemDto.SaleId,
                Quantity = createItemDto.Quantity,
                Price = createItemDto.Price
            };

            // Simpan SaleItem di database
            await saleItemsRepository.CreateAsync(item);

            // 2. Ambil data produk dari Product API untuk mendapatkan stok dengan circuit breaker
            ProductDto product;
            try
            {
                product = await productClient.GetProductByIdAsync(item.ProductId);
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(
                    503,
                    "Product service is temporarily unavailable. Please try again later."
                );
            }

            if (product is null)
            {
                return NotFound("Product not found.");
            }

            // 3. Hitung stok yang baru setelah dikurangi dengan jumlah item yang dijual
            var updatedStockQuantity = product.StockQuantity - item.Quantity;

            if (updatedStockQuantity < 0)
            {
                return BadRequest("Insufficient stock quantity.");
            }

            // 4. Update stok produk menggunakan Product API
            try
            {
                var updateProductStockDto = item.AsUpdateProductStockDto(updatedStockQuantity);
                var stockUpdateResponse = await productClient.UpdateProductStockAsync(
                    item.ProductId,
                    updateProductStockDto
                );

                if (!stockUpdateResponse.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)stockUpdateResponse.StatusCode,
                        "Failed to update product stock."
                    );
                }
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(
                    503,
                    "Product service is temporarily unavailable. Please try again later."
                );
            }

            // 5. Update TotalAmount di Sales
            await UpdateTotalAmountAsync(item.SaleId);

            // 6. Kirim pesan ke RabbitMQ
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                "saleitems-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            string message =
                $"Nama Produk: {product.ProductName}, Jumlah: {item.Quantity}, Harga: {item.Price}, Total Harga: {item.Price * item.Quantity}";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "saleitems-queue",
                body: body
            );

            Console.WriteLine($" [x] Sent: {message}");

            // 7. Kembalikan hasil SaleItem yang baru dibuat
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateSaleItemsDto updateItemDto)
        {
            var existingItem = await saleItemsRepository.GetByIdAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            // Ambil data produk lama
            ProductDto product;
            try
            {
                product = await productClient.GetProductByIdAsync(existingItem.ProductId);
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(
                    503,
                    "Product service is temporarily unavailable. Please try again later."
                );
            }

            if (product is null)
            {
                return NotFound("Product not found.");
            }

            // Tambahkan kembali stok dari item lama sebelum dikurangi lagi
            var restoredStock = product.StockQuantity + existingItem.Quantity;

            // Hitung stok setelah dikurangi dengan kuantitas baru
            var updatedStockQuantity = restoredStock - updateItemDto.Quantity;
            if (updatedStockQuantity < 0)
            {
                return BadRequest("Insufficient stock quantity.");
            }

            // Update item
            existingItem.ProductId = updateItemDto.ProductId;
            existingItem.SaleId = updateItemDto.SaleId;
            existingItem.Quantity = updateItemDto.Quantity;
            existingItem.Price = updateItemDto.Price;

            await saleItemsRepository.UpdateAsync(existingItem);

            // Update stok produk di Product API
            try
            {
                var updateProductStockDto = existingItem.AsUpdateProductStockDto(
                    updatedStockQuantity
                );
                var stockUpdateResponse = await productClient.UpdateProductStockAsync(
                    existingItem.ProductId,
                    updateProductStockDto
                );

                if (!stockUpdateResponse.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)stockUpdateResponse.StatusCode,
                        "Failed to update product stock."
                    );
                }
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(
                    503,
                    "Product service is temporarily unavailable. Please try again later."
                );
            }

            // Update total amount di sale
            await UpdateTotalAmountAsync(existingItem.SaleId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var item = await saleItemsRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            // Ambil data produk dari Product API untuk mendapatkan stok
            var product = await productClient.GetProductByIdAsync(item.ProductId);
            if (product is null)
            {
                return NotFound("Product not found.");
            }

            // Hitung stok setelah ditambahkan kembali
            var updatedStockQuantity = product.StockQuantity + item.Quantity;
            // Update stok produk menggunakan Product API
            var updateProductStockDto = item.AsUpdateProductStockDto(updatedStockQuantity);
            var stockUpdateResponse = await productClient.UpdateProductStockAsync(
                item.ProductId,
                updateProductStockDto
            );

            if (!stockUpdateResponse.IsSuccessStatusCode)
            {
                return StatusCode(
                    (int)stockUpdateResponse.StatusCode,
                    "Failed to update product stock."
                );
            }

            // Hapus item dari database
            await saleItemsRepository.RemoveAsync(id);

            // 🔄 Update TotalAmount
            await UpdateTotalAmountAsync(item.SaleId);

            return NoContent();
        }

        // 🔧 Helper method untuk hitung total
        private async Task UpdateTotalAmountAsync(Guid saleId)
        {
            var saleItems = (await saleItemsRepository.GetAllAsync()).Where(i =>
                i.SaleId == saleId
            );

            var totalAmount = saleItems.Sum(i => i.Quantity * i.Price);

            var sale = await salesRepository.GetByIdAsync(saleId);
            if (sale is not null)
            {
                sale.TotalAmount = totalAmount;
                await salesRepository.UpdateAsync(sale);
            }
        }
    }
}
