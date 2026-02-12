namespace IdentityService.Application;

public record RegisterRequest(string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(Guid UserId, string FullName, string Email, string Token);
