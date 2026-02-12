using Microsoft.EntityFrameworkCore;
using ProductService.Application;
using ProductService.Domain;
using ProductService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default")!,
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default")!)));

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
    options.AddPolicy("frontend", p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors("frontend");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapGet("/health", () => Results.Ok(new { service = "product", status = "ok" }));
app.MapGet("/api/products", async (ProductDbContext db) => Results.Ok(await db.Products.OrderByDescending(x => x.CreatedAt).ToListAsync()));

app.MapPost("/api/products", async (ProductRequest request, ProductDbContext db) =>
{
    var product = new Product
    {
        Name = request.Name,
        Category = request.Category,
        Price = request.Price,
        Description = request.Description
    };

    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapPut("/api/products/{id:guid}", async (Guid id, ProductRequest request, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = request.Name;
    product.Category = request.Category;
    product.Price = request.Price;
    product.Description = request.Description;

    await db.SaveChangesAsync();
    return Results.Ok(product);
});

app.MapDelete("/api/products/{id:guid}", async (Guid id, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
