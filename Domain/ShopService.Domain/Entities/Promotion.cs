using ShopService.Domain.Base;
using ShopService.Domain.Exceptions;
using ShopService.ValueObjects;

namespace ShopService.Domain.Entities;

/// <summary>
/// Акция/скидка продавца.
/// </summary>
public class Promotion : Entity<Guid>
{
    private readonly ICollection<ProductPromotion> _productPromotions = [];

    public Title Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public Discount? Discount { get; private set; }
    public Seller Seller { get; private set; } = default!;
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public IReadOnlyCollection<ProductPromotion> ProductPromotions => _productPromotions.ToList().AsReadOnly();

    protected Promotion()
    {
    }

    internal Promotion(
        Seller seller,
        Title title,
        string? description,
        Discount? discount,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
        : this(Guid.NewGuid(), seller, title, description, discount, startDateUtc, endDateUtc)
    {
    }

    protected Promotion(
        Guid id,
        Seller seller,
        Title title,
        string? description,
        Discount? discount,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
        : base(id)
    {
        Seller = seller ?? throw new ArgumentNullValueException(nameof(seller));
        Title = title ?? throw new ArgumentNullValueException(nameof(title));
        Description = description;
        Discount = discount;

        ValidateDates(startDateUtc, endDateUtc);
        StartDate = EnsureUtc(startDateUtc);
        EndDate = EnsureUtc(endDateUtc);
    }

    internal bool Edit(
        Title title,
        string? description,
        Discount? discount,
        DateTime? startDateUtc,
        DateTime? endDateUtc
    )
    {
        if (title is null) throw new ArgumentNullValueException(nameof(title));

        ValidateDates(startDateUtc, endDateUtc);

        var changed =
            Title != title
            || Description != description
            || Discount != discount
            || StartDate != EnsureUtc(startDateUtc)
            || EndDate != EnsureUtc(endDateUtc);

        if (!changed) return false;

        Title = title;
        Description = description;
        Discount = discount;
        StartDate = EnsureUtc(startDateUtc);
        EndDate = EnsureUtc(endDateUtc);
        return true;
    }

    internal bool End(DateTime endDateUtc)
    {
        ValidateDates(StartDate, endDateUtc);
        var utcEnd = EnsureUtc(endDateUtc);
        if (EndDate == utcEnd) return false;
        EndDate = utcEnd;
        return true;
    }

    internal ProductPromotion AddProduct(Product product)
    {
        if (product is null) throw new ArgumentNullValueException(nameof(product));

        if (_productPromotions.Any(x => x.Product == product))
            throw new InvalidOperationException(
                $"Товар «{product.Name.Value}» (id = {product.Id}) уже привязан к акции «{Title.Value}» (id = {Id})."
            );

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
