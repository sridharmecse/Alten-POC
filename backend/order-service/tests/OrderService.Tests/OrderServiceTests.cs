using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

public class OrderServiceTests
{
    private static OrderDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new OrderDbContext(options);
    }

    [Fact]
    public async Task PlaceOrder_Succeeds_WhenStockSufficient()
    {
        var db = NewDb();
        var mockProduct = new Mock<ProductClient>(new HttpClient(), Mock.Of<ILogger<ProductClient>>());
        mockProduct.Setup(p => p.GetProductAsync(1, default)).ReturnsAsync(new ProductClient.ProductDto(1, "Laptop", 1000m, 5, null));
        mockProduct.Setup(p => p.DecrementStockAsync(1, 2, default)).ReturnsAsync(true);

        var svc = new OrdersDomainService(db, mockProduct.Object);
        var order = await svc.PlaceOrderAsync(new Order{ CustomerName = "Alice", Items = new(){ new OrderItem{ ProductId = 1, Qty = 2 } } });
        order.Status.Should().Be("Confirmed");
        (await db.Orders.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task PlaceOrder_Throws_WhenInsufficientStock()
    {
        var db = NewDb();
        var mockProduct = new Mock<ProductClient>(new HttpClient(), Mock.Of<ILogger<ProductClient>>());
        mockProduct.Setup(p => p.GetProductAsync(1, default)).ReturnsAsync(new ProductClient.ProductDto(1, "Laptop", 1000m, 1, null));

        var svc = new OrdersDomainService(db, mockProduct.Object);
        var act = async () => await svc.PlaceOrderAsync(new Order{ CustomerName = "Bob", Items = new(){ new OrderItem{ ProductId = 1, Qty = 2 } } });
        (await Assert.ThrowsAsync<InvalidOperationException>(act)).Message.Should().Contain("Insufficient stock");
    }

    [Fact]
    public async Task PlaceOrder_Throws_WhenProductMissing()
    {
        var db = NewDb();
        var mockProduct = new Mock<ProductClient>(new HttpClient(), Mock.Of<ILogger<ProductClient>>());
        mockProduct.Setup(p => p.GetProductAsync(5, default)).ReturnsAsync((ProductClient.ProductDto?)null);

        var svc = new OrdersDomainService(db, mockProduct.Object);
        var act = async () => await svc.PlaceOrderAsync(new Order{ CustomerName = "Carol", Items = new(){ new OrderItem{ ProductId = 5, Qty = 1 } } });
        (await Assert.ThrowsAsync<InvalidOperationException>(act)).Message.Should().Contain("not found");
    }
}
