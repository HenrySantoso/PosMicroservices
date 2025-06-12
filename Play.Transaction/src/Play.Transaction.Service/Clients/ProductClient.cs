using System.Net.Http.Json;
using Play.Transaction.Service.Dtos;
using Polly;
using Polly.CircuitBreaker;

namespace Play.Transaction.Service.Clients
{
    public class ProductClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public ProductClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5, // 50% failure rate
                    samplingDuration: TimeSpan.FromSeconds(10), // waktu pengamatan
                    minimumThroughput: 5, // minimal 5 request dalam window
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, breakDelay, context) =>
                    {
                        Console.WriteLine(
                            $"[CircuitBreaker OPEN] Breaking for {breakDelay.TotalSeconds}s due to: {exception.Message}"
                        );
                    },
                    onReset: (context) =>
                    {
                        Console.WriteLine(
                            "[CircuitBreaker CLOSED] Circuit closed. Requests allowed again."
                        );
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("[CircuitBreaker HALF-OPEN] Trial request is allowed.");
                    }
                );
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductsAsync()
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
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

            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
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
            try
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetAsync($"/api/Products/{productId}");
                    Console.WriteLine(
                        $"[CB OK] GET /api/Products/{productId} => {response.StatusCode}"
                    );

                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadFromJsonAsync<ProductDto>();

                    return null;
                });
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("[CB BLOCKED] Request blocked karena circuit OPEN.");
                return null;
            }
        }

        public async Task<HttpResponseMessage> UpdateProductStockAsync(
            Guid productId,
            UpdateProductStockDto updateProductStockDto
        )
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
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
