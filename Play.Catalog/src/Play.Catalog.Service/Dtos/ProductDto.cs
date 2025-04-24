using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    public record ProductDto(
        Guid Id,
        string ProductName,
        string Description,
        decimal Price,
        int StockQuantity,
        Guid CategoryId
    );

    public record CreateProductDto(
        [Required] string ProductName,
        [Required] string Description,
        [Required] decimal Price,
        [Required] int StockQuantity,
        [Required] Guid CategoryId
    );

    public record UpdateProductDto(
        [Required] string ProductName,
        [Required] string Description,
        [Required] decimal Price,
        [Required] int StockQuantity,
        [Required] Guid CategoryId
    );

    public record UpdateProductStockDto(
        [Required] Guid ProductId,
        [Required] int StockQuantity // nilai bisa positif (restock) atau negatif (penjualan)
    );
}
