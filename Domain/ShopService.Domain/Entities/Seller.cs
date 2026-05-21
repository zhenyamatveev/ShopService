using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Продавец управляет своим каталогом товаров и акциями.
/// </summary>
public class Seller : Entity<Guid>
{
    private readonly ICollection<Product> _products = [];
    private readonly ICollection<Promotion> _promotions = [];

    public Name Name { get; private set; } = default!;

    public IReadOnlyCollection<Product> Products => _products.ToList().AsReadOnly();

    public IReadOnlyCollection<Promotion> Promotions => _promotions.ToList().AsReadOnly();

    protected Seller()
    {
    }

    public Seller(Name name)
        : this(Guid.NewGuid(), name)
    {
    }

    protected Seller(Guid id, Name name)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullValueException(nameof(name));
    }

    /// <summary>
    /// Создаёт товар, принадлежащий продавцу.
    /// </summary>
    public Product CreateProduct(Name name, Description? description, Price price)
    {
        if (name is null) throw new ArgumentNullValueException(nameof(name));
        if (price is null) throw new ArgumentNullValueException(nameof(price));

        var product = new Product(this, name, description, price);
        _products.Add(product);

        return product;
    }

    /// <summary>
    /// Редактирует свой товар. <paramref name="actor"/> — продавец, выполняющий действие.
    /// </summary>
    public bool EditProduct(Seller actor, Product product, Name name, Description? description, Price price)
    {
        if (actor is null) throw new ArgumentNullValueException(nameof(actor));
        if (product is null) throw new ArgumentNullValueException(nameof(product));

        if (!ReferenceEquals(actor, this))
            throw new AnotherSellerEditProductException(product, actor);

        if (product.Seller != this)
            throw new AnotherSellerEditProductException(product, actor);

        if (!_products.Contains(product))
            throw new ProductNotBelongSellerException(product, this);

        return product.Edit(name, description, price);
    }

    /// <summary>
    /// Создаёт акцию продавца.
    /// </summary>
    public Promotion CreatePromotion(
        Title title,
        string? description,
        Discount? discount,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
    {
        if (title is null) throw new ArgumentNullValueException(nameof(title));

        var promotion = new Promotion(this, title, description, discount, startDateUtc, endDateUtc);
        _promotions.Add(promotion);

        return promotion;
    }

    /// <summary>
    /// Редактирует свою акцию.
    /// </summary>
    public bool EditPromotion(
        Seller actor,
        Promotion promotion,
        Title title,
        string? description,
        Discount? discount,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
    {
        if (actor is null) throw new ArgumentNullValueException(nameof(actor));
        if (promotion is null) throw new ArgumentNullValueException(nameof(promotion));

        if (!ReferenceEquals(actor, this))
            throw new AnotherSellerEditPromotionException(promotion, actor);

        if (promotion.Seller != this)
            throw new AnotherSellerEditPromotionException(promotion, actor);

        if (!_promotions.Contains(promotion))
            throw new PromotionNotBelongSellerException(promotion, this);

        return promotion.Edit(title, description, discount, startDateUtc, endDateUtc);
    }

    /// <summary>
    /// Завершает свою акцию.
    /// </summary>
    public bool EndPromotion(Seller actor, Promotion promotion, DateTime endDateUtc)
    {
        if (actor is null) throw new ArgumentNullValueException(nameof(actor));
        if (promotion is null) throw new ArgumentNullValueException(nameof(promotion));

        if (!ReferenceEquals(actor, this))
            throw new AnotherSellerEditPromotionException(promotion, actor);

        if (promotion.Seller != this)
            throw new AnotherSellerEditPromotionException(promotion, actor);

        if (!_promotions.Contains(promotion))
            throw new PromotionNotBelongSellerException(promotion, this);

        return promotion.End(endDateUtc);
    }

    /// <summary>
    /// Привязывает свой товар к своей акции (m:n через product_promotions).
    /// </summary>
    public ProductPromotion AddProductToPromotion(Seller actor, Product product, Promotion promotion)
    {
        if (actor is null) throw new ArgumentNullValueException(nameof(actor));
        if (product is null) throw new ArgumentNullValueException(nameof(product));
        if (promotion is null) throw new ArgumentNullValueException(nameof(promotion));

        if (!ReferenceEquals(actor, this))
            throw new InvalidOperationException($"Продавец «{actor.Name.Value}» не может привязать товар к акции от имени другого продавца.");

        if (product.Seller != this)
            throw new AnotherSellerEditProductException(product, actor);

        if (promotion.Seller != this)
            throw new AnotherSellerEditPromotionException(promotion, actor);

        if (!_products.Contains(product))
            throw new ProductNotBelongSellerException(product, this);

        if (!_promotions.Contains(promotion))
            throw new PromotionNotBelongSellerException(promotion, this);

        return promotion.AddProduct(product);
    }
}
