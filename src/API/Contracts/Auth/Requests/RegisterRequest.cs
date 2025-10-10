using Users.Domain.Enums;

namespace API.Contracts.Auth.Requests
{
    public record RegisterRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        UserRole Role
    );
}
