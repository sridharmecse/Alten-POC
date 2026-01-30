using OrderService.Data;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrdersDomainService
    {
        private readonly OrderDbContext _db;
        private readonly ProductClient _productClient;
        public OrdersDomainService(OrderDbContext db, ProductClient productClient)
        {
            _db = db; _productClient = productClient;
        }

        public async Task<Order> PlaceOrderAsync(Order order, CancellationToken ct = default)
        {
            if (order.Items == null || order.Items.Count == 0)
                throw new InvalidOperationException("Order must have at least one item");

            // Validate each item against Product Service
            foreach (var item in order.Items)
            {
                var product = await _productClient.GetProductAsync(item.ProductId, ct);
                if (product is null)
                    throw new InvalidOperationException($"Product {item.ProductId} not found");
                if (item.Qty <= 0)
                    throw new InvalidOperationException("Qty must be > 0");
                if (product.StockQty < item.Qty)
                    throw new InvalidOperationException($"Insufficient stock for product {product.ProductId}");
            }

            // All good -> decrement stocks (simple demo)
            foreach (var item in order.Items)
            {
                var ok = await _productClient.DecrementStockAsync(item.ProductId, item.Qty, ct);
                if (!ok) throw new InvalidOperationException($"Failed to decrement stock for product {item.ProductId}");
            }

            order.OrderDate = DateTime.UtcNow;
            order.Status = "Confirmed";
            _db.Orders.Add(order);
            await _db.SaveChangesAsync(ct);
            return order;
        }
    }
}
