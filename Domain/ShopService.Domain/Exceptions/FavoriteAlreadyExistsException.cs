using ShopService.Domain.Entities;

namespace ShopService.Domain.Exceptions;

public class FavoriteAlreadyExistsException(Customer customer, Product product)
    : InvalidOperationException(
        $"Покупатель «{customer.Name.Value}» уже добавил в избранное товар «{product.Name.Value}» (id = {product.Id}).")
{
    public Customer Customer => customer;
    public Product Product => product;
}
