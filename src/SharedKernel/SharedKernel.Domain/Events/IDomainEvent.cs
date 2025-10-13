using MediatR;

namespace SharedKernel.Domain.Events
{
    public interface IDomainEvent : INotification
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}
