using Microsoft.AspNetCore.Mvc;
using Play.Base.Service.Interfaces;
using Play.Customer.Service.Dtos;
using Play.Customer.Service.Entities;
using Play.Customer.Service.Services;

namespace Play.Customer.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<CustomerItem> customerRepository;

        public CustomersController(IRepository<CustomerItem> customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = (await customerRepository.GetAllAsync()).Select(c => c.AsDto());
            return customers;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetByIdAsync(Guid id)
        {
            var item = await customerRepository.GetByIdAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Post(CreateCustomerDto createItemDto)
        {
            var item = new CustomerItem
            {
                Id = Guid.NewGuid(),
                CustomerName = createItemDto.CustomerName,
                ContactNumber = createItemDto.ContactNumber,
                Email = createItemDto.Email,
                Address = createItemDto.Address
            };
            await customerRepository.CreateAsync(item);
            var customerDto = item.AsDto();
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateCustomerDto updateItemDto)
        {
            var existingItem = await customerRepository.GetByIdAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.CustomerName = updateItemDto.CustomerName;
            existingItem.ContactNumber = updateItemDto.ContactNumber;
            existingItem.Email = updateItemDto.Email;
            existingItem.Address = updateItemDto.Address;
            await customerRepository.UpdateAsync(existingItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var item = await customerRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await customerRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}
