using SharedKernel.Application.Abstractions;
using Users.Application.DTOs;

namespace Users.Application.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
}
