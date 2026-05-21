using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Infrastructure.EntityFramework;
using ShopService.ValueObjects;
using ShopService.WebHost.Contracts.Products;
using ShopService.WebHost.Helpers;

namespace ShopService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = context.Products.AsNoTracking().Include(x => x.Seller).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x => x.Name.Value.Contains(s));
        }

        var items = await query.ToListAsync(cancellationToken);
        return Ok(items.Select(x => new
        {
            x.Id,
            Name = x.Name.Value,
            Description = x.Description?.Value,
            Price = x.Price.Value,
            SellerId = EF.Property<Guid>(x, "SellerId"),
            SellerName = x.Seller.Name.Value
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await context.Products.AsNoTracking().Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return NotFound();

        return Ok(new
        {
            item.Id,
            Name = item.Name.Value,
            Description = item.Description?.Value,
            Price = item.Price.Value,
            SellerId = EF.Property<Guid>(item, "SellerId"),
            SellerName = item.Seller.Name.Value
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var seller = await context.Sellers.FirstOrDefaultAsync(x => x.Id == request.SellerId, cancellationToken);
        if (seller is null) return NotFound(new { message = "Seller not found." });

        var product = seller.CreateProduct(
            new Name(request.Name),
            ValueObjectMapping.ToDescription(request.Description),
            new Price(request.Price));
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, new
        {
            product.Id,
            Name = product.Name.Value,
            Description = product.Description?.Value,
            Price = product.Price.Value,
            SellerId = seller.Id
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var seller = await context.Sellers
            .Include("_products")
            .FirstOrDefaultAsync(x => x.Id == request.SellerId, cancellationToken);
        if (seller is null) return NotFound(new { message = "Seller not found." });

        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null) return NotFound();

        seller.EditProduct(
            seller,
            product,
            new Name(request.Name),
            ValueObjectMapping.ToDescription(request.Description),
            new Price(request.Price));
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            product.Id,
            Name = product.Name.Value,
            Description = product.Description?.Value,
            Price = product.Price.Value,
            SellerId = seller.Id
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null) return NotFound();

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
