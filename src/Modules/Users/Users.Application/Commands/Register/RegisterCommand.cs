using SharedKernel.Application.Abstractions;
using Users.Application.DTOs;
using Users.Domain.Enums;

namespace Users.Application.Commands.Register
{
    public record RegisterCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        UserRole Role
    ) : ICommand<AuthTokenDto>;
}
