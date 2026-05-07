using ShopService.ValueObjects.Base;
using ShopService.ValueObjects.Validators;

namespace ShopService.ValueObjects;

/// <summary>
/// Скидка (процент).
/// </summary>
public sealed class Discount(decimal discount)
    : ValueObject<decimal>(new DiscountValidator(), discount);

