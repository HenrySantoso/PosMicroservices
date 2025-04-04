using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    public record CategoryDto(Guid Id, string CategoryName);

    public record CreateCategoryDto([Required] string CategoryName);

    public record UpdateCategoryDto([Required] string CategoryName);
}
