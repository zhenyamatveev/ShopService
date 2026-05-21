using ShopService.ValueObjects.Base;
using ShopService.ValueObjects.Validators;

namespace ShopService.ValueObjects;

/// <summary>
/// Описание товара (Value Object).
/// </summary>
public sealed class Description(string description)
    : ValueObject<string>(new DescriptionValidator(), description.Trim());
