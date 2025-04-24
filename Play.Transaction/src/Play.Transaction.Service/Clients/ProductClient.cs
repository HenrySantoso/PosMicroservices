using Play.Transaction.Service.Dtos;

namespace Play.Transaction.Service.Clients
{
    public class ProductClient
    {
        private readonly HttpClient _httpClient;

        public ProductClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductsAsync()
        {
            var products = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<ProductDto>>(
                "/api/Products"
            );

            return products ?? new List<ProductDto>();
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(
            IEnumerable<Guid> productIds
        )
        {
            var query = string.Join(",", productIds);
            var response = await _httpClient.GetAsync($"/api/Products?ids={query}");
            Console.WriteLine($"GET /api/Products?ids={query} => {response.StatusCode}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>()
                    ?? Enumerable.Empty<ProductDto>();

            return Enumerable.Empty<ProductDto>();
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            var response = await _httpClient.GetAsync($"/api/Products/{productId}");
            Console.WriteLine($"GET /api/Products/{productId} => {response.StatusCode}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ProductDto>();

            return null;
        }

        // public async Task<HttpResponseMessage> UpdateProductStockAsync(
        //     UpdateProductStockDto updateProductStockDto
        // )
        // {
        //     var response = await _httpClient.PutAsJsonAsync(
        //         $"/api/Products/{updateProductStockDto.ProductId}/stock",
        //         updateProductStockDto
        //     );
        //     Console.WriteLine(
        //         $"PUT /api/Products/{updateProductStockDto.ProductId}/stock => {response.StatusCode}"
        //     );

        //     return response;
        // }

        public async Task<HttpResponseMessage> UpdateProductStockAsync(
            Guid productId,
            UpdateProductStockDto updateProductStockDto
        )
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"/api/Products/{productId}/stock",
                updateProductStockDto
            );
            Console.WriteLine($"PUT /api/Products/{productId}/stock => {response.StatusCode}");

            return response;
        }
    }
}
