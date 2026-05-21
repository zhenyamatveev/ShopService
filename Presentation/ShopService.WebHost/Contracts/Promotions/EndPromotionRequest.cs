namespace ShopService.WebHost.Contracts.Promotions;

public record EndPromotionRequest(Guid SellerId, DateTime EndDateUtc);
