using FluentValidation;

namespace Orders.Application.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item")
                .Must(items => items != null && items.Count > 0).WithMessage("Order must have at least one item");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Product ID is required");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0")
                    .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000");
            });
        }
    }
}
