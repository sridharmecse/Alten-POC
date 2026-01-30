using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDbContext _db;
        private readonly Services.BlobStorageService _blob;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductDbContext db, Services.BlobStorageService blob, ILogger<ProductsController> logger)
        {
            _db = db; _blob = blob; _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll() => await _db.Products.AsNoTracking().ToListAsync();

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var p = await _db.Products.FindAsync(id);
            return p is null ? NotFound() : p;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.ProductId) return BadRequest();
            _db.Entry(product).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p is null) return NotFound();
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id:int}/image")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UploadImage(int id, IFormFile file, CancellationToken ct)
        {
            var product = await _db.Products.FindAsync(id);
            if (product is null) return NotFound();
            if (file is null || file.Length == 0) return BadRequest("No file");
            var (blobName, url) = await _blob.UploadAsync(file.FileName, file.OpenReadStream(), file.ContentType, ct);
            product.BlobName = blobName;
            product.ImageUrl = url;
            await _db.SaveChangesAsync(ct);
            return Ok(new { imageUrl = url });
        }

        [HttpPost("{id:int}/decrement")]
        public async Task<IActionResult> DecrementStock(int id, [FromQuery] int qty)
        {
            if (qty <= 0) return BadRequest("qty must be > 0");
            var p = await _db.Products.FindAsync(id);
            if (p is null) return NotFound();
            if (p.StockQty < qty) return BadRequest("insufficient stock");
            p.StockQty -= qty;
            await _db.SaveChangesAsync();
            return Ok(new { p.ProductId, p.StockQty });
        }
    }
}
