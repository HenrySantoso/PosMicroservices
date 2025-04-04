namespace Play.Transaction.Service.Dtos
{
    public record SalesDto(Guid Id, Guid CustomerId, DateTime SaleDate, decimal TotalAmount);

    public record CreateSalesDto(Guid CustomerId, DateTime SaleDate, decimal TotalAmount);

    public record UpdateSalesDto(Guid CustomerId, DateTime SaleDate, decimal TotalAmount);
}
