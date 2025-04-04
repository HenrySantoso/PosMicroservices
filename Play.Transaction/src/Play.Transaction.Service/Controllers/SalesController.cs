using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Transaction.Service.Dtos;
using Play.Transaction.Service.Entities;

namespace Play.Transaction.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IRepository<Sales> salesRepository;

        public SalesController(IRepository<Sales> salesRepository)
        {
            this.salesRepository = salesRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<SalesDto>> GetAllAsync()
        {
            var sales = (await salesRepository.GetAllAsync()).Select(c => c.AsDto());
            return sales;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesDto>> GetByIdAsync(Guid id)
        {
            var item = await salesRepository.GetByIdAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<SalesDto>> Post(CreateSalesDto createItemDto)
        {
            var item = new Sales
            {
                Id = Guid.NewGuid(),
                CustomerId = createItemDto.CustomerId,
                SaleDate = createItemDto.SaleDate,
                TotalAmount = createItemDto.TotalAmount
            };
            await salesRepository.CreateAsync(item);
            var customerDto = item.AsDto();
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateSalesDto updateItemDto)
        {
            var existingItem = await salesRepository.GetByIdAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.CustomerId = updateItemDto.CustomerId;
            existingItem.SaleDate = updateItemDto.SaleDate;
            existingItem.TotalAmount = updateItemDto.TotalAmount;
            await salesRepository.UpdateAsync(existingItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var item = await salesRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await salesRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
