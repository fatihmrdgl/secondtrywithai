using System.Security.Cryptography;
using System.Text;
using IdentityService.Application;
using IdentityService.Domain;
using IdentityService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default")!,
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default")!)));

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("frontend");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapGet("/health", () => Results.Ok(new { service = "identity", status = "ok" }));

app.MapPost("/api/auth/register", async (RegisterRequest request, IdentityDbContext db) =>
{
    var exists = await db.Users.AnyAsync(x => x.Email == request.Email);
    if (exists) return Results.BadRequest(new { message = "Bu e-posta zaten kayıtlı." });

    var user = new AppUser
    {
        FullName = request.FullName,
        Email = request.Email,
        PasswordHash = Hash(request.Password)
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new AuthResponse(user.Id, user.FullName, user.Email, BuildToken(user)));
});

app.MapPost("/api/auth/login", async (LoginRequest request, IdentityDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
    if (user is null || user.PasswordHash != Hash(request.Password))
    {
        return Results.BadRequest(new { message = "E-posta veya parola hatalı." });
    }

    return Results.Ok(new AuthResponse(user.Id, user.FullName, user.Email, BuildToken(user)));
});

app.MapGet("/api/users", async (IdentityDbContext db) =>
{
    var users = await db.Users
        .Select(x => new { x.Id, x.FullName, x.Email, x.CreatedAt })
        .ToListAsync();

    return Results.Ok(users);
});

app.Run();

static string Hash(string raw)
{
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
    return Convert.ToBase64String(bytes);
}

static string BuildToken(AppUser user)
{
    var tokenRaw = $"{user.Id}:{user.Email}:{DateTime.UtcNow:O}";
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenRaw));
}
