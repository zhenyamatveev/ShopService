using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Domain.Entities;
using ShopService.Infrastructure.EntityFramework;
using ShopService.ValueObjects;
using ShopService.WebHost.Contracts.Customers;

namespace ShopService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await context.Customers.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(items.Select(x => new
        {
            x.Id,
            Name = x.Name.Value
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return NotFound();

        return Ok(new { item.Id, Name = item.Name.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = new Customer(new Name(request.Name));
        context.Customers.Add(customer);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, new { customer.Id, Name = customer.Name.Value });
    }
}

