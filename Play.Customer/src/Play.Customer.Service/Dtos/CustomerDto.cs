using System.ComponentModel.DataAnnotations;

namespace Play.Customer.Service.Dtos
{
    public class CustomerDto
    {
        public record GetCustomerDto(
            Guid CustomerId,
            string CustomerName,
            string ContactNumber,
            string Email,
            string Address
        );

        public record CreateCustomerDto(
            [Required] string CustomerName,
            [Required] string ContactNumber,
            [Required] string Email,
            [Required] string Address
        );

        public record UpdateCustomerDto(
            [Required] string CustomerName,
            [Required] string ContactNumber,
            [Required] string Email,
            [Required] string Address
        );
    }
}
