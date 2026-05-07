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
        var items = await context.Promotions.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(items.Select(x => new
        {
            x.Id,
            Title = x.Title.Value,
            x.Description,
            Discount = x.Discount == null ? (decimal?)null : x.Discount.Value,
            x.SellerId,
            x.StartDate,
            x.EndDate
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var item = await context.Promotions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return NotFound();

        return Ok(new
        {
            item.Id,
            Title = item.Title.Value,
            item.Description,
            Discount = item.Discount == null ? (decimal?)null : item.Discount.Value,
            item.SellerId,
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
            request.EndDateUtc
        );

        context.Promotions.Add(promotion);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, new
        {
            promotion.Id,
            Title = promotion.Title.Value,
            promotion.Description,
            Discount = promotion.Discount == null ? (decimal?)null : promotion.Discount.Value,
            promotion.SellerId,
            promotion.StartDate,
            promotion.EndDate
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePromotionRequest request, CancellationToken cancellationToken)
    {
        var promotion = await context.Promotions.Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (promotion is null) return NotFound();

        Discount? discount = request.Discount is null ? null : new Discount(request.Discount.Value);
        promotion.Edit(
            new Title(request.Title),
            request.Description,
            discount,
            request.StartDateUtc,
            request.EndDateUtc
        );

        await context.SaveChangesAsync(cancellationToken);
        return Ok(new
        {
            promotion.Id,
            Title = promotion.Title.Value,
            promotion.Description,
            Discount = promotion.Discount == null ? (decimal?)null : promotion.Discount.Value,
            promotion.SellerId,
            promotion.StartDate,
            promotion.EndDate
        });
    }

    [HttpPost("{id:int}/end")]
    public async Task<IActionResult> End([FromRoute] int id, [FromBody] EndPromotionRequest request, CancellationToken cancellationToken)
    {
        var promotion = await context.Promotions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (promotion is null) return NotFound();

        promotion.End(request.EndDateUtc);
        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpPost("{id:int}/attach-product")]
    public async Task<IActionResult> AttachProduct([FromRoute] int id, [FromBody] AttachProductRequest request, CancellationToken cancellationToken)
    {
        var promotion = await context.Promotions.Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (promotion is null) return NotFound(new { message = "Promotion not found." });

        var seller = await context.Sellers.FirstAsync(x => x.Id == promotion.SellerId, cancellationToken);

        var product = await context.Products.Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);
        if (product is null) return NotFound(new { message = "Product not found." });

        var link = seller.AddProductToPromotion(product, promotion);
        context.ProductPromotions.Add(link);
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new { link.Id, link.ProductId, link.PromotionId });
    }
}

