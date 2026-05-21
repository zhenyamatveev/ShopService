using ShopService.ValueObjects.Base;
using ShopService.ValueObjects.Validators;

namespace ShopService.ValueObjects;

/// <summary>
/// Имя (для покупателя/продавца/товара).
/// </summary>
public sealed class Name(string name)
    : ValueObject<string>(new NameValidator(), (name ?? string.Empty).Trim());

