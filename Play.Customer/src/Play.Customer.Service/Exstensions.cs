using Play.Customer.Service.Dtos;
using Play.Customer.Service.Entities;

namespace Play.Customer.Service
{
    public static class Exstensions
    {
        public static CustomerDto AsDto(this CustomerItem customer)
        {
            return new CustomerDto(
                customer.Id,
                customer.CustomerName,
                customer.ContactNumber,
                customer.Email,
                customer.Address
            );
        }
    }
}
