using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = (await categoryRepository.GetAllAsync()).Select(c => c.AsDto());
            return categories;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetByIdAsync(Guid id)
        {
            var item = await categoryRepository.GetByIdAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Post(CreateCategoryDto createItemDto)
        {
            var item = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = createItemDto.CategoryName
            };
            await categoryRepository.CreateAsync(item);
            var customerDto = item.AsDto();
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateCategoryDto updateItemDto)
        {
            var existingItem = await categoryRepository.GetByIdAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.CategoryName = updateItemDto.CategoryName;
            await categoryRepository.UpdateAsync(existingItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var item = await categoryRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await categoryRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
