using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Transaction.Service.Clients;
using Play.Transaction.Service.Dtos;
using Play.Transaction.Service.Entities;

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

        // [HttpGet]
        // public async Task<IEnumerable<SaleItemsDto>> GetAllAsync()
        // {
        //     var saleItems = (await saleItemsRepository.GetAllAsync()).Select(c => c.AsDto());
        //     return saleItems;
        // }

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

        // [HttpGet("{id}")]
        // public async Task<ActionResult<SaleItemsDto>> GetByIdAsync(Guid id)
        // {
        //     var item = await saleItemsRepository.GetByIdAsync(id);
        //     if (item is null)
        //     {
        //         return NotFound();
        //     }
        //     return item.AsDto();
        // }

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

        // [HttpPost]
        // public async Task<ActionResult<SaleItemsDto>> Post(CreateSaleItemsDto createItemDto)
        // {
        //     var item = new SaleItems
        //     {
        //         Id = Guid.NewGuid(),
        //         ProductId = createItemDto.ProductId,
        //         SaleId = createItemDto.SaleId,
        //         Quantity = createItemDto.Quantity,
        //         Price = createItemDto.Price
        //     };

        //     await saleItemsRepository.CreateAsync(item);

        //     // update stock quantity from product service

        //     // 🔄 Update TotalAmount di tabel Sales
        //     await UpdateTotalAmountAsync(item.SaleId);

        //     return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        // }

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

            // 2. Ambil data produk dari Product API untuk mendapatkan stok
            var product = await productClient.GetProductByIdAsync(item.ProductId);
            if (product is null)
            {
                return NotFound("Product not found.");
            }

            // 3. Hitung stok yang baru setelah dikurangi dengan jumlah item yang dijual
            var updatedStockQuantity = product.StockQuantity - item.Quantity;

            // Cek apakah stok cukup
            if (updatedStockQuantity < 0)
            {
                return BadRequest("Insufficient stock quantity.");
            }

            // 4. Update stok produk menggunakan Product API
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

            // 5. Update TotalAmount di Sales (opsional, jika perlu)
            await UpdateTotalAmountAsync(item.SaleId);

            // 6. Kembalikan hasil SaleItem yang baru dibuat
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
            var product = await productClient.GetProductByIdAsync(existingItem.ProductId);
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
            var updateProductStockDto = existingItem.AsUpdateProductStockDto(updatedStockQuantity);
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
