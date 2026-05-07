namespace ShopService.WebHost.Contracts.Promotions;

public record CreatePromotionRequest(
    string Title,
    string? Description,
    decimal? Discount,
    int SellerId,
    DateTime? StartDateUtc,
    DateTime? EndDateUtc
);

