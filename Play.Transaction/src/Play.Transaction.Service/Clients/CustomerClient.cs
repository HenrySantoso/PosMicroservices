using System.Net.Http.Json;
using Play.Transaction.Service.Dtos;
using Polly;
using Polly.CircuitBreaker;

namespace Play.Transaction.Service.Clients
{
    public class CustomerClient
    {
        private readonly HttpClient httpClient;
        private readonly AsyncCircuitBreakerPolicy circuitBreakerPolicy;

        public CustomerClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;

            circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5, // 50% dari request gagal
                    samplingDuration: TimeSpan.FromSeconds(10), // dalam 10 detik
                    minimumThroughput: 10, // minimal 10 request
                    durationOfBreak: TimeSpan.FromSeconds(30), // circuit break selama 30 detik
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

        public async Task<IReadOnlyCollection<CustomerDto>> GetCustomersAsync()
        {
            return await circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var customers = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CustomerDto>>(
                    "/api/Customer"
                );
                return customers ?? new List<CustomerDto>();
            });
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId)
        {
            return await circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient.GetAsync($"/api/Customers/{customerId}");
                Console.WriteLine($"GET /api/Customers/{customerId} => {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<CustomerDto>();

                return null;
            });
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersByIdsAsync(
            IEnumerable<Guid> customerIds
        )
        {
            return await circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var query = string.Join(",", customerIds);
                var response = await httpClient.GetAsync($"/api/Customers?ids={query}");
                Console.WriteLine($"GET /api/Customers?ids={query} => {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>()
                        ?? Enumerable.Empty<CustomerDto>();

                return Enumerable.Empty<CustomerDto>();
            });
        }
    }
}
