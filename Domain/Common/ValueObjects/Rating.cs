using System.Collections.Generic;

namespace ApiApplication.Domain.Common.Models
{

    public sealed class Rating : ValueObject
    {
        public Rating(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public static Rating Create(int value)
        {
            // TODO: Enforce invariants
            return new Rating(value);
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

}