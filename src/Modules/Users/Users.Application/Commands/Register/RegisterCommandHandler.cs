using SharedKernel.Application.Abstractions;
using Users.Application.Abstractions;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.Register
{
    public class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthTokenDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;

        public RegisterCommandHandler(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher, 
            IJwtService jwtService, 
            IUnitOfWork unitOfWork, 
            ITenantProvider tenantProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
        }
        public async Task<Result<AuthTokenDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var email = new Email(request.Email);

                // Check if user with the same email already exists
                var existingUser = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
                if (existingUser)
                {
                    return Result<AuthTokenDto>.Failure("User with the same email already exists.");
                }

                // Hash the password
                var passwordHash = _passwordHasher.HashPassword(request.Password);

                // Create new user
                var user = User.Create(
                    request.FirstName,
                    request.LastName,
                    email,
                    passwordHash,
                    request.Role,
                    _tenantProvider.GetCurrentTenantId()
                );

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshTokenString = _jwtService.GenerateRefreshToken();
                var refreshToken = new RefreshToken(refreshTokenString, DateTime.UtcNow.AddDays(7));

                user.AddRefreshToken(refreshToken);
                user.RecordLogin();

                await _userRepository.AddAsync(user, cancellationToken);
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
