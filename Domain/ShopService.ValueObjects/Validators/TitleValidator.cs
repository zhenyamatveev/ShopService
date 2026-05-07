using ShopService.ValueObjects.Base;
using ShopService.ValueObjects.Exceptions;

namespace ShopService.ValueObjects.Validators;

public class TitleValidator : IValidator<string>
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 150;

    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        if (value.Length < MIN_LENGTH)
            throw new ArgumentShortValueException(nameof(value), MIN_LENGTH, value.Length);

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), MAX_LENGTH, value.Length);
    }
}

