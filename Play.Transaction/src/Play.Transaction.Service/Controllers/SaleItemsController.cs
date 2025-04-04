using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Transaction.Service.Dtos;
using Play.Transaction.Service.Entities;

namespace Play.Transaction.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleItemsController : ControllerBase
    {
        private readonly IRepository<SaleItems> saleItemsRepository;

        public SaleItemsController(IRepository<SaleItems> saleItemsRepository)
        {
            this.saleItemsRepository = saleItemsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<SaleItemsDto>> GetAllAsync()
        {
            var saleItems = (await saleItemsRepository.GetAllAsync()).Select(c => c.AsDto());
            return saleItems;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleItemsDto>> GetByIdAsync(Guid id)
        {
            var item = await saleItemsRepository.GetByIdAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<SaleItemsDto>> Post(CreateSaleItemsDto createItemDto)
        {
            var item = new SaleItems
            {
                Id = Guid.NewGuid(),
                ProductId = createItemDto.ProductId,
                SaleId = createItemDto.SaleId,
                Quantity = createItemDto.Quantity,
                Price = createItemDto.Price
            };
            await saleItemsRepository.CreateAsync(item);
            var customerDto = item.AsDto();
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

            existingItem.ProductId = updateItemDto.ProductId;
            existingItem.SaleId = updateItemDto.SaleId;
            existingItem.Quantity = updateItemDto.Quantity;
            existingItem.Price = updateItemDto.Price;
            await saleItemsRepository.UpdateAsync(existingItem);
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

            await saleItemsRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
