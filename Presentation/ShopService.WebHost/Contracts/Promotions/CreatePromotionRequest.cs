namespace ShopService.WebHost.Contracts.Promotions;

public record CreatePromotionRequest(
    string Title,
    string? Description,
    decimal? Discount,
    Guid SellerId,
    DateTime? StartDateUtc,
    DateTime? EndDateUtc
);
