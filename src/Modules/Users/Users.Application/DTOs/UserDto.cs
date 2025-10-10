namespace Users.Application.DTOs
{
    public record UserDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive,
        DateTime? LastLoginAt,
        DateTime CreatedAt
    );
}
