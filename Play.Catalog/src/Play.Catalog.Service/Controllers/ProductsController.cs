using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> productRepository;

        public ProductsController(IRepository<Product> productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = (await productRepository.GetAllAsync()).Select(c => c.AsDto());
            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetByIdAsync(Guid id)
        {
            var item = await productRepository.GetByIdAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Post(CreateProductDto createItemDto)
        {
            if (createItemDto.CategoryId == Guid.Empty)
            {
                return BadRequest("CategoryId is required.");
            }

            var item = new Product
            {
                Id = Guid.NewGuid(),
                ProductName = createItemDto.ProductName,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                StockQuantity = createItemDto.StockQuantity,
                CategoryId = createItemDto.CategoryId
            };
            await productRepository.CreateAsync(item);
            var customerDto = item.AsDto();
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateProductDto updateItemDto)
        {
            if (updateItemDto.CategoryId == Guid.Empty)
            {
                return BadRequest("CategoryId is required.");
            }

            var existingItem = await productRepository.GetByIdAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.ProductName = updateItemDto.ProductName;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;
            existingItem.StockQuantity = updateItemDto.StockQuantity;
            existingItem.CategoryId = updateItemDto.CategoryId;
            await productRepository.UpdateAsync(existingItem);
            return NoContent();
        }

        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(Guid id, UpdateProductStockDto dto)
        {
            if (dto.StockQuantity <= 0)
            {
                return BadRequest("QuantityStock must be greater than 0.");
            }

            var product = await productRepository.GetByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            // 🔻 Perbarui Stock
            product.StockQuantity = dto.StockQuantity;

            await productRepository.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var item = await productRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await productRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
