using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Domain.Entities;
using ShopService.Domain.Exceptions;
using ShopService.Infrastructure.EntityFramework;
using ShopService.ValueObjects;
using ShopService.WebHost.Contracts.Products;

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
            x.Description,
            Price = x.Price.Value,
            x.SellerId
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var item = await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return NotFound();

        return Ok(new
        {
            item.Id,
            Name = item.Name.Value,
            item.Description,
            Price = item.Price.Value,
            item.SellerId
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var seller = await context.Sellers.FirstOrDefaultAsync(x => x.Id == request.SellerId, cancellationToken);
        if (seller is null) return NotFound(new { message = "Seller not found." });

        var product = seller.CreateProduct(new Name(request.Name), request.Description, new Price(request.Price));
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, new
        {
            product.Id,
            Name = product.Name.Value,
            product.Description,
            Price = product.Price.Value,
            product.SellerId
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await context.Products.Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null) return NotFound();

        product.Edit(new Name(request.Name), request.Description, new Price(request.Price));
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            product.Id,
            Name = product.Name.Value,
            product.Description,
            Price = product.Price.Value,
            product.SellerId
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null) return NotFound();

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}

