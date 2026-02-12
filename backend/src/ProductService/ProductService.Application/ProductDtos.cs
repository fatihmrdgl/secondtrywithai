namespace ProductService.Application;

public record ProductRequest(string Name, string Category, decimal Price, string Description);
