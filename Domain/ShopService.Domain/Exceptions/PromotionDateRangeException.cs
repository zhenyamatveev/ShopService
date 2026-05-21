namespace ShopService.Domain.Exceptions;

public class PromotionDateRangeException(DateTime? startDate, DateTime? endDate)
    : ArgumentException($"Invalid promotion date range. startDate={startDate:O}, endDate={endDate:O}");

