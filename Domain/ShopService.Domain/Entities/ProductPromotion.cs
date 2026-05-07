using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;

namespace ShopService.Domain.Entities;

/// <summary>
/// Связка товара и акции (m:n).
/// </summary>
public class ProductPromotion : Entity<int>
{
    public int ProductId { get; private set; }
    public Product Product { get; private set; }

    public int PromotionId { get; private set; }
    public Promotion Promotion { get; private set; }

    private ProductPromotion(int id, int productId, int promotionId)
        : base(id)
    {
        ProductId = productId;
        PromotionId = promotionId;
        Product = null!;
        Promotion = null!;
    }

    protected ProductPromotion()
    {
        Product = null!;
        Promotion = null!;
    }

    internal ProductPromotion(Product product, Promotion promotion)
        : this(default, product?.Id ?? default, promotion?.Id ?? default)
    {
        if (product is null) throw new ArgumentNullValueException(nameof(product));
        if (promotion is null) throw new ArgumentNullValueException(nameof(promotion));

        Product = product;
        Promotion = promotion;
    }
}

