namespace Users.Application.DTOs
{
    public record AuthTokenDto(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        UserDto User
    );
}
