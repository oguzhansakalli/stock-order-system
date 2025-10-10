namespace API.Contracts.Auth.Responses
{
    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        UserResponse User
    );

    public record UserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive
    );
}
