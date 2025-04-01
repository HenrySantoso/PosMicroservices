namespace Play.Customer.Service.Dtos;

public record CustomerDto(
    Guid Id,
    int CustomerId,
    string CustomerName,
    string ContactNumber,
    string Email,
    string Address
);

public record CreateCustomerDto(
    string CustomerName,
    string ContactNumber,
    string Email,
    string Address
);

public record UpdateCustomerDto(
    string CustomerName,
    string ContactNumber,
    string Email,
    string Address
);
