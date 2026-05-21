namespace ShopService.ValueObjects.Exceptions;

public class ArgumentShortValueException(string paramName, int minLength, int actualLength)
    : ArgumentException(
        $"Argument \"{paramName}\" value is too short. Min length: {minLength}, actual: {actualLength}.",
        paramName
    );

