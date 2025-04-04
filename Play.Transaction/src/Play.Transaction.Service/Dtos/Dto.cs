namespace Play.Transaction.Service.Dtos
{
    public record CustomerDto(
        Guid Id,
        string CustomerName,
        string ContactNumber,
        string Address,
        string Email
    );

    public record ProductDto(
        Guid Id,
        Guid CategoryId,
        string ProductName,
        decimal Price,
        int StockQuantity,
        string Description
    );

    public record CategoryDto(Guid Id, string CategoryName);
}
