using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;

namespace ShopService.Domain.Entities;

/// <summary>
/// Избранное (связка покупатель — товар).
/// </summary>
public class Favorite : Entity<Guid>
{
    public Customer Customer { get; private set; } = default!;
    public Product Product { get; private set; } = default!;

    protected Favorite()
    {
    }

    internal Favorite(Customer customer, Product product)
        : this(Guid.NewGuid(), customer, product)
    {
    }

    private Favorite(Guid id, Customer customer, Product product)
        : base(id)
    {
        Customer = customer ?? throw new ArgumentNullValueException(nameof(customer));
        Product = product ?? throw new ArgumentNullValueException(nameof(product));
    }
}
