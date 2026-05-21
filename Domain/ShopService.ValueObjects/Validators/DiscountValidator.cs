using ShopService.ValueObjects.Base;

namespace ShopService.ValueObjects.Validators;

public class DiscountValidator : IValidator<decimal>
{
    // decimal(5,2): 3 цифры до точки и 2 после точки.
    public static decimal MAX_VALUE => 999.99m;
    public static int MAX_SCALE => 2;

    public static decimal MIN_DISCOUNT => 0.01m;
    public static decimal MAX_DISCOUNT => 100m;

    public void Validate(decimal value)
    {
        if (value < MIN_DISCOUNT || value > MAX_DISCOUNT)
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Discount must be between {MIN_DISCOUNT} and {MAX_DISCOUNT}.");

        if (value > MAX_VALUE)
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Discount must be <= {MAX_VALUE}.");

        if (GetScale(value) > MAX_SCALE)
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Discount scale must be <= {MAX_SCALE}.");
    }

    private static int GetScale(decimal value)
        => (decimal.GetBits(value)[3] >> 16) & 0x7F;
}

