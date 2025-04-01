using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    public static class CategoryDto
    {
        public record GetCategoryDto(Guid CategoryId, string CategoryName);

        public record CreateCategoryDto([Required] string CategoryName);

        public record UpdateCategoryDto([Required] string CategoryName);
    }
}
