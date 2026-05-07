namespace ShopService.Domain.Exceptions;

public class FavoriteNotFoundException(int customerId, int productId)
    : InvalidOperationException($"Favorite not found. customerId={customerId}, productId={productId}");

