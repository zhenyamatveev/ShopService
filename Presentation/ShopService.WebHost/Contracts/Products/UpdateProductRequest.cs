namespace ShopService.WebHost.Contracts.Products;

public record UpdateProductRequest(
    Guid SellerId,
    string Name,
    string? Description,
    decimal Price
);
