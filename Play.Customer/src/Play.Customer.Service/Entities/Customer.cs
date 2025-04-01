using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Customer.Service.Entities
{
    public class Customer
    {
        Guid CustomerId { get; init; }
        string CustomerName { get; set; } = string.Empty;
        string ContactNumber { get; set; } = string.Empty;
        string Email { get; set; } = string.Empty;
        string Address { get; set; } = string.Empty;
    }
}
