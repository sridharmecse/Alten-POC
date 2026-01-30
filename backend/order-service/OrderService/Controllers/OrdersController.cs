using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _db;
        private readonly OrdersDomainService _svc;
        public OrdersController(OrderDbContext db, OrdersDomainService svc)
        {
            _db = db; _svc = svc;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(Order order)
        {
            var result = await _svc.PlaceOrderAsync(order);
            return CreatedAtAction(nameof(GetById), new { id = result.OrderId }, result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == id);
            return order is null ? NotFound() : order;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> GetAll() => await _db.Orders.Include(o => o.Items).ToListAsync();
    }
}
