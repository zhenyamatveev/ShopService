using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Domain.Entities;
using ShopService.Infrastructure.EntityFramework;
using ShopService.ValueObjects;
using ShopService.WebHost.Contracts.Sellers;

namespace ShopService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellersController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await context.Sellers.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(items.Select(x => new
        {
            x.Id,
            Name = x.Name.Value
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var item = await context.Sellers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return NotFound();

        return Ok(new { item.Id, Name = item.Name.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSellerRequest request, CancellationToken cancellationToken)
    {
        var seller = new Seller(new Name(request.Name));
        context.Sellers.Add(seller);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = seller.Id }, new { seller.Id, Name = seller.Name.Value });
    }
}

