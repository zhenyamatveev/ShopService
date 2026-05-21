using ShopService.ValueObjects.Base;
using ShopService.ValueObjects.Validators;

namespace ShopService.ValueObjects;

/// <summary>
/// Заголовок акции.
/// </summary>
public sealed class Title(string title)
    : ValueObject<string>(new TitleValidator(), (title ?? string.Empty).Trim());

