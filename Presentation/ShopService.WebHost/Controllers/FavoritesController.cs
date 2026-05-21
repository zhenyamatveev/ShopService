using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Infrastructure.EntityFramework;
using ShopService.WebHost.Contracts.Favorites;

namespace ShopService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("by-customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomer([FromRoute] Guid customerId, CancellationToken cancellationToken)
    {
        var favorites = await context.Favorites
            .AsNoTracking()
            .Include(x => x.Product)
            .ThenInclude(p => p.Seller)
            .Where(x => EF.Property<Guid>(x, "CustomerId") == customerId)
            .ToListAsync(cancellationToken);

        return Ok(favorites.Select(x => new
        {
            x.Id,
            CustomerId = EF.Property<Guid>(x, "CustomerId"),
            ProductId = x.Product.Id,
            Product = new
            {
                x.Product.Id,
                Name = x.Product.Name.Value,
                Description = x.Product.Description?.Value,
                Price = x.Product.Price.Value,
                SellerId = EF.Property<Guid>(x.Product, "SellerId"),
                SellerName = x.Product.Seller.Name.Value
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

        return Ok(new
        {
            favorite.Id,
            CustomerId = request.CustomerId,
            ProductId = product.Id
        });
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(
        [FromQuery] Guid customerId,
        [FromQuery] Guid productId,
        CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .Include("_favorites")
            .FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        if (customer is null) return NotFound(new { message = "Customer not found." });

        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);
        if (product is null) return NotFound(new { message = "Product not found." });

        customer.RemoveFromFavorites(product);

        var favoriteRow = await context.Favorites
            .FirstOrDefaultAsync(
                x => EF.Property<Guid>(x, "CustomerId") == customerId && EF.Property<Guid>(x, "ProductId") == productId,
                cancellationToken);

        if (favoriteRow is not null)
        {
            context.Favorites.Remove(favoriteRow);
            await context.SaveChangesAsync(cancellationToken);
        }

        return NoContent();
    }
}
