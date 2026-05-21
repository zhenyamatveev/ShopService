using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Товар, размещённый продавцом.
/// </summary>
public class Product : Entity<Guid>
{
    private readonly ICollection<Favorite> _favorites = [];
    private readonly ICollection<ProductPromotion> _productPromotions = [];

    public Name Name { get; private set; } = default!;
    public Description? Description { get; private set; }
    public Price Price { get; private set; } = default!;
    public Seller Seller { get; private set; } = default!;

    public IReadOnlyCollection<Favorite> Favorites => _favorites.ToList().AsReadOnly();

    public IReadOnlyCollection<ProductPromotion> ProductPromotions => _productPromotions.ToList().AsReadOnly();

    protected Product()
    {
    }

    internal Product(Seller seller, Name name, Description? description, Price price)
        : this(Guid.NewGuid(), seller, name, description, price)
    {
    }

    protected Product(Guid id, Seller seller, Name name, Description? description, Price price)
        : base(id)
    {
        Seller = seller ?? throw new ArgumentNullValueException(nameof(seller));
        Name = name ?? throw new ArgumentNullValueException(nameof(name));
        Price = price ?? throw new ArgumentNullValueException(nameof(price));
        Description = description;
    }

    internal bool Edit(Name name, Description? description, Price price)
    {
        if (name is null) throw new ArgumentNullValueException(nameof(name));
        if (price is null) throw new ArgumentNullValueException(nameof(price));

        var changed = Name != name || Description != description || Price != price;
        if (!changed) return false;

        Name = name;
        Description = description;
        Price = price;
        return true;
    }
}
