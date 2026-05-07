namespace ShopService.WebHost.Contracts.Promotions;

public record UpdatePromotionRequest(
    string Title,
    string? Description,
    decimal? Discount,
    DateTime? StartDateUtc,
    DateTime? EndDateUtc
);

