using ShopService.Domain.Entities;

namespace ShopService.Domain.Exceptions;

public class FavoriteNotFoundException(Customer customer, Product product)
    : InvalidOperationException(
        $"У покупателя «{customer.Name.Value}» нет в избранном товара «{product.Name.Value}» (id = {product.Id}).")
{
    public Customer Customer => customer;
    public Product Product => product;
}
