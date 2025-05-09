using Play.Transaction.Service.Dtos;

namespace Play.Transaction.Service.Clients
{
    public class CustomerClient
    {
        private readonly HttpClient httpClient;

        public CustomerClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CustomerDto>> GetCustomersAsync()
        {
            var customers = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CustomerDto>>(
                "/api/Customer"
            );
            return customers ?? new List<CustomerDto>();
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid customerId)
        {
            var response = await httpClient.GetAsync($"/api/Customers/{customerId}");
            Console.WriteLine($"GET /api/Customers/{customerId} => {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CustomerDto>();
            }

            return null;
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersByIdsAsync(
            IEnumerable<Guid> customerIds
        )
        {
            var query = string.Join(",", customerIds);
            var response = await httpClient.GetAsync($"/api/Customers?ids={query}");
            Console.WriteLine($"GET /api/Customers?ids={query} => {response.StatusCode}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>()
                    ?? Enumerable.Empty<CustomerDto>();

            return Enumerable.Empty<CustomerDto>();
        }
    }
}
