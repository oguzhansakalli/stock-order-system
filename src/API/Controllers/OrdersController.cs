using API.Contracts.Orders.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands.CancelOrder;
using Orders.Application.Queries.GetOrderById;
using Orders.Application.Queries.GetOrders;
using Orders.Application.Queries.GetOrdersByCustomer;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
        {
            var query = new GetOrdersQuery();
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Get orders by customer ID
        /// </summary>
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrdersByCustomer(
            Guid customerId,
            CancellationToken cancellationToken)
        {
            var query = new GetOrdersByCustomerQuery(customerId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { error = "Invalid token" });

            var command = new Orders.Application.Commands.CreateOrder.CreateOrderCommand(
                userId, // Use logged-in user as customer
                request.CustomerName,
                request.Notes,
                request.Items.Select(i => new Orders.Application.Commands.CreateOrder.OrderItemRequest(
                    i.ProductId,
                    i.Quantity
                )).ToList()
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = result.Value.Id },
                result.Value);
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        [HttpPatch("{id:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(
            Guid id,
            [FromBody] CancelOrderRequest? request,
            CancellationToken cancellationToken)
        {
            var command = new CancelOrderCommand(id, request?.Reason);
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Order cancelled successfully" });
        }
    }
}
