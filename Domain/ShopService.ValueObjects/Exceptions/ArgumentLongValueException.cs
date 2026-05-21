namespace ShopService.ValueObjects.Exceptions;

public class ArgumentLongValueException(string paramName, int maxLength, int actualLength)
    : ArgumentException(
        $"Argument \"{paramName}\" value is too long. Max length: {maxLength}, actual: {actualLength}.",
        paramName
    );

