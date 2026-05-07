namespace ShopService.Domain.Exceptions;

public class FavoriteAlreadyExistsException(int customerId, int productId)
    : InvalidOperationException($"Favorite already exists. customerId={customerId}, productId={productId}");

