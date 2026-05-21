using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Infrastructure.EntityFramework;
using ShopService.ValueObjects;
using ShopService.WebHost.Contracts.Promotions;

namespace ShopService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await context.Promotions.AsNoTracking().Include(x => x.Seller).ToListAsync(cancellationToken);
        return Ok(items.Select(x => new
        {
            x.Id,
            Title = x.Title.Value,
            x.Description,
            Discount = x.Discount == null ? (decimal?)null : x.Discount.Value,
            SellerId = EF.Property<Guid>(x, "SellerId"),
            x.StartDate,
            x.EndDate
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await context.Promotions.AsNoTracking().Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return NotFound();

        return Ok(new
        {
            item.Id,
            Title = item.Title.Value,
            item.Description,
            Discount = item.Discount == null ? (decimal?)null : item.Discount.Value,
            SellerId = EF.Property<Guid>(item, "SellerId"),
            item.StartDate,
            item.EndDate
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        var seller = await context.Sellers.FirstOrDefaultAsync(x => x.Id == request.SellerId, cancellationToken);
        if (seller is null) return NotFound(new { message = "Seller not found." });

        Discount? discount = request.Discount is null ? null : new Discount(request.Discount.Value);
        var promotion = seller.CreatePromotion(
            new Title(request.Title),
            request.Description,
            discount,
            request.StartDateUtc,
            request.EndDateUtc);

        context.Promotions.Add(promotion);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, new
        {
            promotion.Id,
            Title = promotion.Title.Value,
            promotion.Description,
            Discount = promotion.Discount == null ? (decimal?)null : promotion.Discount.Value,
            SellerId = seller.Id,
            promotion.StartDate,
            promotion.EndDate
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePromotionRequest request, CancellationToken cancellationToken)
    {
        var seller = await context.Sellers
            .Include("_promotions")
            .FirstOrDefaultAsync(x => x.Id == request.SellerId, cancellationToken);
        if (seller is null) return NotFound(new { message = "Seller not found." });

        var promotion = await context.Promotions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (promotion is null) return NotFound();

        Discount? discount = request.Discount is null ? null : new Discount(request.Discount.Value);
        seller.EditPromotion(
            seller,
            promotion,
            new Title(request.Title),
            request.Description,
            discount,
            request.StartDateUtc,
            request.EndDateUtc);

        await context.SaveChangesAsync(cancellationToken);
        return Ok(new
        {
            promotion.Id,
            Title = promotion.Title.Value,
            promotion.Description,
            Discount = promotion.Discount == null ? (decimal?)null : promotion.Discount.Value,
            SellerId = seller.Id,
            promotion.StartDate,
            promotion.EndDate
        });
    }

    [HttpPost("{id:guid}/end")]
    public async Task<IActionResult> End(
        [FromRoute] Guid id,
        [FromBody] EndPromotionRequest request,
        CancellationToken cancellationToken)
    {
        var seller = await context.Sellers
            .Include("_promotions")
            .FirstOrDefaultAsync(x => x.Id == request.SellerId, cancellationToken);
        if (seller is null) return NotFound(new { message = "Seller not found." });

        var promotion = await context.Promotions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (promotion is null) return NotFound();

        seller.EndPromotion(seller, promotion, request.EndDateUtc);
        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpPost("{id:guid}/attach-product")]
    public async Task<IActionResult> AttachProduct([FromRoute] Guid id, [FromBody] AttachProductRequest request, CancellationToken cancellationToken)
    {
        var seller = await context.Sellers
            .Include("_products")
            .Include("_promotions")
            .FirstOrDefaultAsync(x => x.Id == request.SellerId, cancellationToken);
        if (seller is null) return NotFound(new { message = "Seller not found." });

        var promotion = await context.Promotions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (promotion is null) return NotFound(new { message = "Promotion not found." });

        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);
        if (product is null) return NotFound(new { message = "Product not found." });

        var link = seller.AddProductToPromotion(seller, product, promotion);
        context.ProductPromotions.Add(link);
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            link.Id,
            ProductId = product.Id,
            PromotionId = promotion.Id
        });
    }
}
