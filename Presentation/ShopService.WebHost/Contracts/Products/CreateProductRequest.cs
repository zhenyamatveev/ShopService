namespace ShopService.WebHost.Contracts.Products;

public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid SellerId
);
