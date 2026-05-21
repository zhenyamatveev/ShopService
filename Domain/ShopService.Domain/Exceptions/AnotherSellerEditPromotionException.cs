using ShopService.Domain.Entities;

namespace ShopService.Domain.Exceptions;

public class AnotherSellerEditPromotionException(Promotion promotion, Seller actor)
    : InvalidOperationException(
        $"Продавец «{actor.Name.Value}» не может изменить акцию «{promotion.Title.Value}», " +
        $"владелец — «{promotion.Seller.Name.Value}» (id акции = {promotion.Id}).")
{
    public Promotion Promotion => promotion;
    public Seller Actor => actor;
}
