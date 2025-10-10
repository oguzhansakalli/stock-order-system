using SharedKernel.Domain.ValueObjects;

namespace Users.Domain.ValueObjects
{
    public class RefreshToken : ValueObject
    {
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRevoked { get; private set; }
        private RefreshToken() { } // For EF Core
        public RefreshToken(string token, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty.", nameof(token));
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            IsRevoked = false;
        }
        public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid() => !IsRevoked && !IsExpired();
        public void Revoke()
        {
            IsRevoked = true;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Token;
            yield return ExpiresAt;
        }
    }
}
