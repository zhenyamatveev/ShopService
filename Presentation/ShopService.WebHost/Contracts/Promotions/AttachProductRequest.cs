namespace ShopService.WebHost.Contracts.Promotions;

public record AttachProductRequest(Guid SellerId, Guid ProductId);
