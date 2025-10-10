using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Abstractions;
using Users.Application.Abstractions;
using Users.Application.DTOs;
using Users.Domain.Repositories;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthTokenDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher, 
            IJwtService jwtService,
            [FromKeyedServices("Users")] IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<AuthTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var email = new Email(request.Email);

                // Get user
                var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
                if (user == null)
                    return Result<AuthTokenDto>.Failure("Invalid email or password");

                // Verify password
                if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                    return Result<AuthTokenDto>.Failure("Invalid email or password");

                // Check if user is active
                if (!user.IsActive)
                    return Result<AuthTokenDto>.Failure("User account is deactivated");

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshTokenString = _jwtService.GenerateRefreshToken();
                var refreshToken = new RefreshToken(refreshTokenString, DateTime.UtcNow.AddDays(7));

                user.AddRefreshToken(refreshToken);
                user.RecordLogin();

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

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

                var authToken = new AuthTokenDto(
                    accessToken,
                    refreshTokenString,
                    DateTime.UtcNow.AddDays(7),
                    userDto
                );

                return Result<AuthTokenDto>.Success(authToken);
            }
            catch (ArgumentException ex)
            {
                return Result<AuthTokenDto>.Failure(ex.Message);
            }
        }
    }
}
