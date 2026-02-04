namespace UserManagementAPI.DTOs;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email
);
