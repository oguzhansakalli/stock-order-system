using Inventory.Domain.Enums;
using SharedKernel.Domain.ValueObjects;

namespace Inventory.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public Currency Currency { get; private set; }
        private Money() { } // For EF Core
        public Money(decimal amount, Currency currency = Currency.TRY)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            return new Money(a.Amount - b.Amount, a.Currency);
        }

        public static Money operator *(Money money, int multiplier)
        {
            return new Money(money.Amount * multiplier, money.Currency);
        }

        public Money ConvertTo(Currency targetCurrency, decimal exchangeRate)
        {
            if (Currency == targetCurrency)
                return this;

            return new Money(Amount * exchangeRate, targetCurrency);
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
