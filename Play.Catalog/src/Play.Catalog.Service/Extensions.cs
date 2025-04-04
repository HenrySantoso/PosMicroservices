using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Exstensions
    {
        public static CategoryDto AsDto(this Category category)
        {
            return new CategoryDto(category.Id, category.CategoryName);
        }

        public static ProductDto AsDto(this Product product)
        {
            return new ProductDto(
                product.Id,
                product.ProductName,
                product.Description,
                product.Price,
                product.CategoryId
            );
        }
    }
}
