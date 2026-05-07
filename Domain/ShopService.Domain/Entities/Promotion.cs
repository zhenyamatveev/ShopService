using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Акция/скидка продавца.
/// </summary>
public class Promotion : Entity<int>
{
    private readonly ICollection<ProductPromotion> _productPromotions = new List<ProductPromotion>();

    public Title Title { get; private set; }
    public string? Description { get; private set; }
    public Discount? Discount { get; private set; }

    public int SellerId { get; private set; }
    public Seller Seller { get; private set; }

    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public IReadOnlyCollection<ProductPromotion> ProductPromotions
        => _productPromotions.ToList().AsReadOnly();

    private Promotion(
        int id,
        Title title,
        string? description,
        Discount? discount,
        int sellerId,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
        : base(id)
    {
        Title = title;
        Description = description;
        Discount = discount;
        SellerId = sellerId;
        Seller = null!;

        StartDate = EnsureUtc(startDateUtc);
        EndDate = EnsureUtc(endDateUtc);
    }

    protected Promotion()
    {
        Title = null!;
        Seller = null!;
    }

    internal Promotion(
        Seller seller,
        Title title,
        string? description,
        Discount? discount,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
        : this(default, title, description, discount, seller?.Id ?? default, startDateUtc, endDateUtc)
    {
        if (seller is null) throw new ArgumentNullValueException(nameof(seller));
        if (title is null) throw new ArgumentNullValueException(nameof(title));

        ValidateDates(startDateUtc, endDateUtc);

        Seller = seller;
    }

    public void Edit(
        Title title,
        string? description,
        Discount? discount,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
    {
        if (title is null) throw new ArgumentNullValueException(nameof(title));

        ValidateDates(startDateUtc, endDateUtc);

        Title = title;
        Description = description;
        Discount = discount;
        StartDate = EnsureUtc(startDateUtc);
        EndDate = EnsureUtc(endDateUtc);
    }

    public void End(DateTime endDateUtc)
    {
        ValidateDates(StartDate, endDateUtc);
        EndDate = EnsureUtc(endDateUtc);
    }

    internal ProductPromotion AddProduct(Product product)
    {
        if (product is null) throw new ArgumentNullValueException(nameof(product));

        if (_productPromotions.Any(x => x.ProductId == product.Id))
            throw new InvalidOperationException($"Product {product.Id} already attached to promotion {Id}.");

        var link = new ProductPromotion(product, this);
        _productPromotions.Add(link);

        return link;
    }

    private static DateTime? EnsureUtc(DateTime? value)
    {
        if (value is null) return null;
        return value.Value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
    }

    private static void ValidateDates(DateTime? startDateUtc, DateTime? endDateUtc)
    {
        if (startDateUtc is null || endDateUtc is null) return;
        if (startDateUtc.Value > endDateUtc.Value)
            throw new PromotionDateRangeException(startDateUtc, endDateUtc);
    }
}

