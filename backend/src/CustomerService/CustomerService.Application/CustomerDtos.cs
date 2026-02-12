namespace CustomerService.Application;

public record CustomerRequest(string FullName, string Phone, string Email, string City);
