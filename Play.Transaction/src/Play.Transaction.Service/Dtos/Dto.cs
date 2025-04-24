using System.ComponentModel.DataAnnotations;

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

    public record UpdateProductStockDto(
        Guid ProductId,
        int StockQuantity // nilai bisa positif (restock) atau negatif (penjualan)
    );

    public record SaleItemsProductDto(
        Guid Id,
        Guid ProductId,
        Guid SaleId,
        string ProductName,
        decimal Price,
        int Quantity
    );

    public record SaleItemsDto(Guid Id, Guid ProductId, Guid SaleId, int Quantity, decimal Price);

    public record CreateSaleItemsDto(
        [Required] Guid ProductId,
        [Required] Guid SaleId,
        [Required] int Quantity,
        [Required] decimal Price
    );

    public record UpdateSaleItemsDto(
        [Required] Guid ProductId,
        [Required] Guid SaleId,
        int Quantity,
        decimal Price
    );

    public record SalesDto(Guid Id, Guid CustomerId, DateTime SaleDate, decimal TotalAmount);

    public record CreateSalesDto(
        [Required] Guid CustomerId,
        DateTime SaleDate,
        decimal TotalAmount
    );

    public record UpdateSalesDto(
        [Required] Guid CustomerId,
        DateTime SaleDate,
        decimal TotalAmount
    );

    public record SaleByIdDto(
        Guid Id,
        Guid CustomerId,
        string CustomerName,
        DateTime SaleDate,
        decimal TotalAmount
    );

    public record SaleDetailDto(
        Guid SaleId,
        Guid CustomerId,
        string CustomerName,
        DateTime SaleDate,
        decimal TotalAmount,
        List<SaleItemDetailDto> SaleItems
    );

    public record SaleItemDetailDto(
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal Price
    );
}
