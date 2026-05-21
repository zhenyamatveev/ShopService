using ShopService.ValueObjects.Base;
using ShopService.ValueObjects.Validators;

namespace ShopService.ValueObjects;

/// <summary>
/// Цена товара.
/// </summary>
public sealed class Price(decimal price)
    : ValueObject<decimal>(new PriceValidator(), price);

