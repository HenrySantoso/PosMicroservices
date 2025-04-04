namespace Play.Transaction.Service.Dtos
{
    public record SaleItemsDto(Guid Id, Guid ProductId, Guid SaleId, int Quantity, decimal Price);

    public record CreateSaleItemsDto(Guid ProductId, Guid SaleId, int Quantity, decimal Price);

    public record UpdateSaleItemsDto(Guid ProductId, Guid SaleId, int Quantity, decimal Price);
}
