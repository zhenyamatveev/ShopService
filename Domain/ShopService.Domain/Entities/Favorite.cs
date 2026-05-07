using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;

namespace ShopService.Domain.Entities;

/// <summary>
/// Избранное (связка покупатель - товар).
/// </summary>
public class Favorite : Entity<int>
{
    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; }

    public int ProductId { get; private set; }
    public Product Product { get; private set; }

    private Favorite(int id, int customerId, int productId)
        : base(id)
    {
        CustomerId = customerId;
        ProductId = productId;
        Customer = null!;
        Product = null!;
    }

    protected Favorite()
    {
        Customer = null!;
        Product = null!;
    }

    internal Favorite(Customer customer, Product product)
        : this(default, customer?.Id ?? default, product?.Id ?? default)
    {
        if (customer is null) throw new ArgumentNullValueException(nameof(customer));
        if (product is null) throw new ArgumentNullValueException(nameof(product));

        Customer = customer;
        Product = product;
    }
}

