using ShopService.Domain.Entities;

namespace ShopService.Domain.Exceptions;

public class PromotionNotBelongSellerException(Promotion promotion, Seller seller)
    : InvalidOperationException(
        $"Акция «{promotion.Title.Value}» (id = {promotion.Id}) не найдена у продавца «{seller.Name.Value}» (id = {seller.Id}).")
{
    public Promotion Promotion => promotion;
    public Seller Seller => seller;
}
