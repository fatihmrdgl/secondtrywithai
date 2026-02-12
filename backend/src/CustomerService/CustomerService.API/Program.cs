using CustomerService.Application;
using CustomerService.Domain;
using CustomerService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomerDbContext>(options =>
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
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapGet("/health", () => Results.Ok(new { service = "customer", status = "ok" }));
app.MapGet("/api/customers", async (CustomerDbContext db) => Results.Ok(await db.Customers.OrderByDescending(x => x.CreatedAt).ToListAsync()));

app.MapPost("/api/customers", async (CustomerRequest request, CustomerDbContext db) =>
{
    var customer = new Customer
    {
        FullName = request.FullName,
        Phone = request.Phone,
        Email = request.Email,
        City = request.City
    };

    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    return Results.Created($"/api/customers/{customer.Id}", customer);
});

app.MapPut("/api/customers/{id:guid}", async (Guid id, CustomerRequest request, CustomerDbContext db) =>
{
    var customer = await db.Customers.FindAsync(id);
    if (customer is null) return Results.NotFound();

    customer.FullName = request.FullName;
    customer.Phone = request.Phone;
    customer.Email = request.Email;
    customer.City = request.City;

    await db.SaveChangesAsync();
    return Results.Ok(customer);
});

app.MapDelete("/api/customers/{id:guid}", async (Guid id, CustomerDbContext db) =>
{
    var customer = await db.Customers.FindAsync(id);
    if (customer is null) return Results.NotFound();

    db.Customers.Remove(customer);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
