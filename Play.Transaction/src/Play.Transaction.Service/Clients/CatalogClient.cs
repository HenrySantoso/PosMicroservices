using Play.Transaction.Service.Dtos;

namespace Pos.Transaction.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient httpClient;

        public CatalogClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CategoryDto>> GetCateogriesAsync()
        {
            var categories = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CategoryDto>>(
                "/api/Cateogry"
            );
            return categories;
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductssAsync()
        {
            var products = await httpClient.GetFromJsonAsync<IReadOnlyCollection<ProductDto>>(
                "/api/Product"
            );
            return products;
        }
    }
}
