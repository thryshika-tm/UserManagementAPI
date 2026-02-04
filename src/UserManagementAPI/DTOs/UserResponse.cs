namespace UserManagementAPI.DTOs;

public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
