using MediatR;
using SharedKernel.Application.Abstractions;
using SharedKernel.Domain.Events;

namespace SharedKernel.Infrastructure.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IPublisher _publisher;

        public DomainEventDispatcher(IPublisher publisher)
        {
            _publisher = publisher;
        }
        public async Task DispatchAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
