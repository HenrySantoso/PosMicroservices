using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    public record ProductDto(
        Guid Id,
        string ProductName,
        string Description,
        decimal Price,
        Guid CategoryId
    );

    public record CreateProductDto(
        [Required] string ProductName,
        [Required] string Description,
        [Required] decimal Price,
        [Required] Guid CategoryId
    );

    public record UpdateProductDto(
        [Required] string ProductName,
        [Required] string Description,
        [Required] decimal Price,
        [Required] Guid CategoryId
    );
}
