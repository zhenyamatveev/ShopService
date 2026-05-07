using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Продавец управляет своим каталогом товаров и акциями.
/// </summary>
public class Seller : Entity<int>
{
    private readonly ICollection<Product> _products = new List<Product>();
    private readonly ICollection<Promotion> _promotions = new List<Promotion>();

    public Name Name { get; private set; }

    public IReadOnlyCollection<Product> Products
        => _products.ToList().AsReadOnly();

    public IReadOnlyCollection<Promotion> Promotions
        => _promotions.ToList().AsReadOnly();

    private Seller(int id, Name name)
        : base(id)
    {
        Name = name;
    }

    protected Seller()
    {
        Name = null!;
    }

    public Seller(Name name)
        : this(default, name)
    {
        if (name is null) throw new ArgumentNullValueException(nameof(name));
    }

    /// <summary>
    /// Создает товар, принадлежащий продавцу.
    /// </summary>
    public Product CreateProduct(Name name, string? description, Price price)
    {
        if (name is null) throw new ArgumentNullValueException(nameof(name));
        if (price is null) throw new ArgumentNullValueException(nameof(price));

        var product = new Product(this, name, description, price);
        _products.Add(product);

        return product;
    }

    /// <summary>
    /// Создает акцию продавца.
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
    /// Привязывает товар к акции (m:n через product_promotions).
    /// </summary>
    public ProductPromotion AddProductToPromotion(Product product, Promotion promotion)
    {
        if (product is null) throw new ArgumentNullValueException(nameof(product));
        if (promotion is null) throw new ArgumentNullValueException(nameof(promotion));

        if (product.SellerId != Id)
            throw new InvalidOperationException($"Product {product.Id} does not belong to seller {Id}.");

        if (promotion.SellerId != Id)
            throw new InvalidOperationException($"Promotion {promotion.Id} does not belong to seller {Id}.");

        return promotion.AddProduct(product);
    }
}

