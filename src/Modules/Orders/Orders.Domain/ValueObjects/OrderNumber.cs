using SharedKernel.Domain.ValueObjects;

namespace Orders.Domain.ValueObjects
{
    public class OrderNumber : ValueObject
    {
        public string Value { get; private set; }
        private OrderNumber() { } // For EF Core
        private OrderNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Order number cannot be null or empty", nameof(value));

            Value = value;
        }
        public static OrderNumber Generate()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return new OrderNumber($"ORD-{timestamp}-{random}");
        }
        public static OrderNumber Create(string value)
        {
            return new OrderNumber(value);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public static implicit operator string(OrderNumber orderNumber) => orderNumber.Value;
        public override string ToString() => Value;
    }
}
