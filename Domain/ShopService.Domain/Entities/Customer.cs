using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Покупатель, который может просматривать каталог и добавлять товары в избранное.
/// </summary>
public class Customer : Entity<int>
{
    private readonly ICollection<Favorite> _favorites = new List<Favorite>();

    public Name Name { get; private set; }

    public IReadOnlyCollection<Favorite> Favorites
        => _favorites.ToList().AsReadOnly();

    private Customer(int id, Name name)
        : base(id)
    {
        Name = name;
    }

    protected Customer()
    {
        Name = null!;
    }

    public Customer(Name name)
        : this(default, name)
    {
        if (name is null) throw new ArgumentNullValueException(nameof(name));
    }

    /// <summary>
    /// Добавляет товар в избранное.
    /// </summary>
    public Favorite AddToFavorites(Product product)
    {
        if (product is null) throw new ArgumentNullValueException(nameof(product));

        if (_favorites.Any(x => x.ProductId == product.Id))
            throw new FavoriteAlreadyExistsException(Id, product.Id);

        var favorite = new Favorite(this, product);
        _favorites.Add(favorite);

        return favorite;
    }

    /// <summary>
    /// Удаляет товар из избранного.
    /// </summary>
    public void RemoveFromFavorites(Product product)
    {
        if (product is null) throw new ArgumentNullValueException(nameof(product));

        var favorite = _favorites.FirstOrDefault(x => x.ProductId == product.Id);
        if (favorite is null)
            throw new FavoriteNotFoundException(Id, product.Id);

        _favorites.Remove(favorite);
    }
}

