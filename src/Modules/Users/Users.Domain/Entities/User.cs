using SharedKernel.Domain.Entities;
using Users.Domain.Enums;
using Users.Domain.Events;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        private User() { } // For EF Core
        public static User Create(
            string firstName, 
            string lastName, 
            Email email, 
            string passwordHash, 
            UserRole role,
            Guid tenantId)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                IsActive = true
            };

            user.SetTenantId(tenantId);
            user.AddDomainEvent(new UserRegisteredEvent(user.Id, email, firstName, lastName, role.ToString()));

            return user;
        }
        public void UpdateProfile(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
        }
        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }
        public void AddRefreshToken(RefreshToken token)
        {
            // Revoke old tokens
            foreach (var oldToken in _refreshTokens.Where(t => t.IsValid()))
            {
                oldToken.Revoke();
            }

            _refreshTokens.Add(token);
        }
        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            AddDomainEvent(new UserLoggedInEvent(Id, Email));
        }
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
        public string FullName => $"{FirstName} {LastName}";
    }
}
