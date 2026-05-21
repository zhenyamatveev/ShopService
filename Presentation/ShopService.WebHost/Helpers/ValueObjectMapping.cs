using ShopService.ValueObjects;

namespace ShopService.WebHost.Helpers;

internal static class ValueObjectMapping
{
    public static Description? ToDescription(string? text)
        => string.IsNullOrWhiteSpace(text) ? null : new Description(text);
}
