namespace UserManagementAPI.DTOs;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email
);
