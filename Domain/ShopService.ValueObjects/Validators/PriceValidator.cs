using ShopService.ValueObjects.Base;

namespace ShopService.ValueObjects.Validators;

public class PriceValidator : IValidator<decimal>
{
    // decimal(10,2): 8 цифр до точки и 2 после точки.
    public static decimal MAX_VALUE => 99999999.99m;
    public static int MAX_SCALE => 2;

    public void Validate(decimal value)
    {
        if (value <= 0m)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Price must be greater than zero.");

        if (value > MAX_VALUE)
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Price must be <= {MAX_VALUE}.");

        if (GetScale(value) > MAX_SCALE)
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Price scale must be <= {MAX_SCALE}.");
    }

    private static int GetScale(decimal value)
        => (decimal.GetBits(value)[3] >> 16) & 0x7F;
}

