namespace ShopService.WebHost.Contracts.Promotions;

public record UpdatePromotionRequest(
    Guid SellerId,
    string Title,
    string? Description,
    decimal? Discount,
    DateTime? StartDateUtc,
    DateTime? EndDateUtc
);
