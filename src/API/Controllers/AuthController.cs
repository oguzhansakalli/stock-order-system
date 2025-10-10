using API.Contracts.Auth.Requests;
using API.Contracts.Auth.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Commands.Login;
using Users.Application.Commands.Register;
using Users.Application.Queries.GetUserById;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.Role
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            var response = new AuthResponse(
                result.Value.AccessToken,
                result.Value.RefreshToken,
                result.Value.ExpiresAt,
                new UserResponse(
                    result.Value.User.Id,
                    result.Value.User.FirstName,
                    result.Value.User.LastName,
                    result.Value.User.Email,
                    result.Value.User.Role,
                    result.Value.User.IsActive
                )
            );

            return CreatedAtAction(nameof(GetMe), response);
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.Email, request.Password);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            var response = new AuthResponse(
                result.Value.AccessToken,
                result.Value.RefreshToken,
                result.Value.ExpiresAt,
                new UserResponse(
                    result.Value.User.Id,
                    result.Value.User.FirstName,
                    result.Value.User.LastName,
                    result.Value.User.Email,
                    result.Value.User.Role,
                    result.Value.User.IsActive
                )
            );

            return Ok(response);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { error = "Invalid token" });

            var query = new GetUserByIdQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            var response = new UserResponse(
                result.Value.Id,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.Email,
                result.Value.Role,
                result.Value.IsActive
            );

            return Ok(response);
        }
    }
}
