using SharedKernel.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Users.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; private set; }
        private Email() { } // For EF Core
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty.", nameof(value));

            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email format", nameof(value));

            Value = value.ToLowerInvariant();
        }
        private static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public static implicit operator string(Email email) => email.Value;
        public override string ToString() => Value;
    }
}
