namespace ShopService.WebHost.Contracts.Favorites;

public record AddFavoriteRequest(Guid CustomerId, Guid ProductId);
