namespace OrderService.Services
{
    public class ProductClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ProductClient> _logger;
        public ProductClient(HttpClient http, ILogger<ProductClient> logger)
        {
            _http = http; _logger = logger;
        }

        public record ProductDto(int ProductId, string Name, decimal Price, int StockQty, string? ImageUrl);

        public async Task<ProductDto?> GetProductAsync(int id, CancellationToken ct = default)
        {
            var res = await _http.GetAsync($"/products/{id}", ct);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct);
        }

        public async Task<bool> DecrementStockAsync(int id, int qty, CancellationToken ct = default)
        {
            var res = await _http.PostAsync($"/products/{id}/decrement?qty={qty}", content: null, ct);
            return res.IsSuccessStatusCode;
        }
    }
}
