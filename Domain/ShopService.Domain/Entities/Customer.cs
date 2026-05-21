using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Покупатель, который может просматривать каталог и добавлять товары в избранное.
/// </summary>
public class Customer : Entity<Guid>
{
    private readonly ICollection<Favorite> _favorites = [];

    public Name Name { get; private set; } = default!;

    public IReadOnlyCollection<Favorite> Favorites => _favorites.ToList().AsReadOnly();

    protected Customer()
    {
    }

    public Customer(Name name)
        : this(Guid.NewGuid(), name)
    {
    }

    protected Customer(Guid id, Name name)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullValueException(nameof(name));
    }

    /// <summary>
    /// Добавляет товар в избранное.
    /// </summary>
    public Favorite AddToFavorites(Product product)
    {
        if (product is null) throw new ArgumentNullValueException(nameof(product));

        if (_favorites.Any(x => x.Product == product))
            throw new FavoriteAlreadyExistsException(this, product);

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

        var favorite = _favorites.FirstOrDefault(x => x.Product == product);
        if (favorite is null)
            throw new FavoriteNotFoundException(this, product);

        _favorites.Remove(favorite);
    }
}
