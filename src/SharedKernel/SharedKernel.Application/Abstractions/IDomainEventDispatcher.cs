using SharedKernel.Domain.Events;

namespace SharedKernel.Application.Abstractions
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
