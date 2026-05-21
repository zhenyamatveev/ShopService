using ShopService.ValueObjects.Base;
using ShopService.ValueObjects.Exceptions;

namespace ShopService.ValueObjects.Validators;

public class DescriptionValidator : IValidator<string>
{
    public const int MAX_LENGTH = 2000;

    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), MAX_LENGTH, value.Length);
    }
}
