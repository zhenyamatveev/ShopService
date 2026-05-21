using ShopService.Domain.Entities;

namespace ShopService.Domain.Exceptions;

public class AnotherSellerEditProductException(Product product, Seller actor)
    : InvalidOperationException(
        $"Продавец «{actor.Name.Value}» не может изменить товар «{product.Name.Value}», " +
        $"владелец — «{product.Seller.Name.Value}» (id товара = {product.Id}).")
{
    public Product Product => product;
    public Seller Actor => actor;
}
