using SharedKernel.Application.Abstractions;
using Users.Application.DTOs;
using Users.Domain.Repositories;

namespace Users.Application.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<UserDto>.Failure("User not found");

            var userDto = new UserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role.ToString(),
                user.IsActive,
                user.LastLoginAt,
                user.CreatedAt
            );

            return Result<UserDto>.Success(userDto);
        }
    }
}
