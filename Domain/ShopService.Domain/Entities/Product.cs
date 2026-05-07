using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Товар, размещенный продавцом.
/// </summary>
public class Product : Entity<int>
{
    private readonly ICollection<Favorite> _favorites = new List<Favorite>();
    private readonly ICollection<ProductPromotion> _productPromotions = new List<ProductPromotion>();

    public Name Name { get; private set; }
    public string? Description { get; private set; }
    public Price Price { get; private set; }

    public int SellerId { get; private set; }
    public Seller Seller { get; private set; }

    public IReadOnlyCollection<Favorite> Favorites
        => _favorites.ToList().AsReadOnly();

    public IReadOnlyCollection<ProductPromotion> ProductPromotions
        => _productPromotions.ToList().AsReadOnly();

    private Product(int id, Name name, string? description, Price price, int sellerId)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        SellerId = sellerId;
        Seller = null!;
    }

    protected Product()
    {
        Name = null!;
        Price = null!;
        Seller = null!;
    }

    internal Product(Seller seller, Name name, string? description, Price price)
        : this(default, name, description, price, seller?.Id ?? default)
    {
        if (seller is null) throw new ArgumentNullValueException(nameof(seller));
        if (name is null) throw new ArgumentNullValueException(nameof(name));
        if (price is null) throw new ArgumentNullValueException(nameof(price));

        Seller = seller;
    }

    /// <summary>
    /// Редактирует данные товара.
    /// </summary>
    public void Edit(Name name, string? description, Price price)
    {
        if (name is null) throw new ArgumentNullValueException(nameof(name));
        if (price is null) throw new ArgumentNullValueException(nameof(price));

        Name = name;
        Description = description;
        Price = price;
    }
}

