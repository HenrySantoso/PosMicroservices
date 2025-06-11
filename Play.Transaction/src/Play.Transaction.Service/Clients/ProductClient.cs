using System.Net.Http.Json;
using Play.Transaction.Service.Dtos;
using Polly;
using Polly.Retry;

namespace Play.Transaction.Service.Clients
{
    public class ProductClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public ProductClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>() // optional: for timeout
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        Console.WriteLine(
                            $"[Polly Retry {retryCount}] {exception.GetType().Name}: {exception.Message}"
                        );
                    }
                );
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductsAsync()
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var products = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<ProductDto>>(
                    "/api/Products"
                );
                return products ?? new List<ProductDto>();
            });
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(
            IEnumerable<Guid> productIds
        )
        {
            var query = string.Join(",", productIds);

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync($"/api/Products?ids={query}");
                Console.WriteLine($"GET /api/Products?ids={query} => {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>()
                        ?? Enumerable.Empty<ProductDto>();

                return Enumerable.Empty<ProductDto>();
            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync($"/api/Products/{productId}");
                Console.WriteLine($"GET /api/Products/{productId} => {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ProductDto>();

                return null;
            });
        }

        public async Task<HttpResponseMessage> UpdateProductStockAsync(
            Guid productId,
            UpdateProductStockDto updateProductStockDto
        )
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"/api/Products/{productId}/stock",
                    updateProductStockDto
                );
                Console.WriteLine($"PUT /api/Products/{productId}/stock => {response.StatusCode}");
                return response;
            });
        }
    }
}
