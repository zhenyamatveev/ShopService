using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;

namespace ShopService.Domain.Entities;

/// <summary>
/// Связка товара и акции (m:n).
/// </summary>
public class ProductPromotion : Entity<Guid>
{
    public Product Product { get; private set; } = default!;
    public Promotion Promotion { get; private set; } = default!;

    protected ProductPromotion()
    {
    }

    internal ProductPromotion(Product product, Promotion promotion)
        : this(Guid.NewGuid(), product, promotion)
    {
    }

    private ProductPromotion(Guid id, Product product, Promotion promotion)
        : base(id)
    {
        Product = product ?? throw new ArgumentNullValueException(nameof(product));
        Promotion = promotion ?? throw new ArgumentNullValueException(nameof(promotion));
    }
}
