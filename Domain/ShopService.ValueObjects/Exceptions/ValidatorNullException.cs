namespace ShopService.ValueObjects.Exceptions;

public class ValidatorNullException(string paramName)
    : ArgumentNullException(paramName, $"Validator \"{paramName}\" value is null");

