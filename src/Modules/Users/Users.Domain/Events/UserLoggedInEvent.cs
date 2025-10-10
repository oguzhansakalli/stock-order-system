using SharedKernel.Domain.Events;

namespace Users.Domain.Events
{
    public sealed class UserLoggedInEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public DateTime LoginAt { get; }
        public UserLoggedInEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
            LoginAt = DateTime.UtcNow;
        }
    }
}
