namespace ProductService.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQty { get; set; }
        public string? ImageUrl { get; set; }
        public string? BlobName { get; set; }
    }
}
