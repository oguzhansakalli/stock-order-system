using SharedKernel.Application.Abstractions;
using Users.Application.DTOs;

namespace Users.Application.Commands.Login
{
    public record LoginCommand(string Email, string Password) : ICommand<AuthTokenDto>;
}
