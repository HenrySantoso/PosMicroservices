using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Transaction.Service.Clients;
using Play.Transaction.Service.Dtos;
using Play.Transaction.Service.Entities;

namespace Play.Transaction.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IRepository<Sales> salesRepository;
        private readonly IRepository<SaleItems> saleItemRepository;
        private readonly HttpClient httpClient;
        private readonly CustomerClient customerClient;
        private readonly ProductClient productClient;

        public SalesController(
            IRepository<Sales> salesRepository,
            IRepository<SaleItems> saleItemRepository,
            HttpClient httpClient,
            CustomerClient customerClient,
            ProductClient productClient
        )
        {
            this.saleItemRepository = saleItemRepository;
            this.salesRepository = salesRepository;
            this.httpClient = httpClient;
            this.customerClient = customerClient;
            this.productClient = productClient;
        }

        [HttpGet]
        public async Task<IEnumerable<SalesDto>> GetAllAsync()
        {
            // Get all sales items
            var saleItems = await saleItemRepository.GetAllAsync();
            // Get sales item by sale id
            var saleItemsBySaleId = saleItems.GroupBy(item => item.SaleId).ToList();
            // looping through each sale item and get the total amount
            foreach (var saleItem in saleItemsBySaleId)
            {
                var totalAmount = saleItem.Sum(item => item.Price * item.Quantity);
                var sale = await salesRepository.GetByIdAsync(saleItem.Key);
                if (sale != null)
                {
                    sale.TotalAmount = totalAmount;
                    await salesRepository.UpdateAsync(sale);
                }
            }

            var sales = await salesRepository.GetAllAsync();
            return sales.Select(sale => sale.AsDto());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleByIdDto>> GetByIdAsync(Guid id)
        {
            var sale = await salesRepository.GetByIdAsync(id);
            if (sale is null)
            {
                return NotFound();
            }

            var customer = await customerClient.GetCustomerByIdAsync(sale.CustomerId);
            if (customer is null)
            {
                return NotFound();
            }

            var saleByIdDto = sale.AsSaleByIdDto(customer.CustomerName);
            return saleByIdDto;
        }


        // [HttpGet("{id}")]
        // public async Task<ActionResult<SaleDetailDto>> GetByIdAsync(Guid id)
        // {
        //     var sale = await salesRepository.GetByIdAsync(id);
        //     if (sale is null)
        //     {
        //         return NotFound();
        //     }

        //     var customer = await customerClient.GetCustomerByIdAsync(sale.CustomerId);
        //     if (customer is null)
        //     {
        //         return NotFound();
        //     }

        //     var saleItems = await saleItemRepository.GetByIdAsync(sale.Id);
        //     if (saleItems is null)
        //     {
        //         return NotFound("Sale items not found.");
        //     }

        //     var product = await productClient.GetProductByIdAsync(saleItems.ProductId);
        //     if (product is null)
        //     {
        //         return NotFound();
        //     }

        //     var saleItemDtos = saleItems.Select(item => item.AsDto()).ToList();

        //     var saleDetailDto = sale.AsSaleDetailDto(
        //         customer.CustomerName,
        //         product.ProductName,
        //         saleItemDtos
        //     );

        //     return saleDetailDto;
        // }

        [HttpPost]
        public async Task<ActionResult<SalesDto>> Post(CreateSalesDto createDto)
        {
            var sale = new Sales
            {
                Id = Guid.NewGuid(),
                CustomerId = createDto.CustomerId,
                SaleDate = createDto.SaleDate,
                TotalAmount = createDto.TotalAmount
            };

            await salesRepository.CreateAsync(sale);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = sale.Id }, sale.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateSalesDto updateDto)
        {
            var existingSale = await salesRepository.GetByIdAsync(id);
            if (existingSale is null)
            {
                return NotFound();
            }

            existingSale.CustomerId = updateDto.CustomerId;
            existingSale.SaleDate = updateDto.SaleDate;
            existingSale.TotalAmount = updateDto.TotalAmount;

            await salesRepository.UpdateAsync(existingSale);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var sale = await salesRepository.GetByIdAsync(id);
            if (sale is null)
            {
                return NotFound();
            }

            await salesRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
