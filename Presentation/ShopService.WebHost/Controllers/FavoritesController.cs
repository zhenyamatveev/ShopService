using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Infrastructure.EntityFramework;
using ShopService.WebHost.Contracts.Favorites;

namespace ShopService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("by-customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomer([FromRoute] int customerId, CancellationToken cancellationToken)
    {
        var favorites = await context.Favorites
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        return Ok(favorites.Select(x => new
        {
            x.Id,
            x.CustomerId,
            x.ProductId,
            Product = new
            {
                x.Product.Id,
                Name = x.Product.Name.Value,
                x.Product.Description,
                Price = x.Product.Price.Value,
                x.Product.SellerId
            }
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddFavoriteRequest request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .Include("_favorites")
            .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        if (customer is null) return NotFound(new { message = "Customer not found." });

        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);
        if (product is null) return NotFound(new { message = "Product not found." });

        var favorite = customer.AddToFavorites(product);
        context.Favorites.Add(favorite);
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new { favorite.Id, favorite.CustomerId, favorite.ProductId });
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromQuery] int customerId, [FromQuery] int productId, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .Include("_favorites")
            .FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        if (customer is null) return NotFound(new { message = "Customer not found." });

        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
        if (product is null) return NotFound(new { message = "Product not found." });

        customer.RemoveFromFavorites(product);

        var favoriteRow = await context.Favorites
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ProductId == productId, cancellationToken);

        if (favoriteRow is not null)
        {
            context.Favorites.Remove(favoriteRow);
            await context.SaveChangesAsync(cancellationToken);
        }

        return NoContent();
    }
}

