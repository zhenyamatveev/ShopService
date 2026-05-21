using ShopService.Domain.Entities;

namespace ShopService.Domain.Exceptions;

public class ProductNotBelongSellerException(Product product, Seller seller)
    : InvalidOperationException(
        $"Товар «{product.Name.Value}» (id = {product.Id}) не найден в каталоге продавца «{seller.Name.Value}» (id = {seller.Id}).")
{
    public Product Product => product;
    public Seller Seller => seller;
}
