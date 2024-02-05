namespace ApiApplication.Domain.Common.Models
{
    public sealed class AverageRating : ValueObject
    {
        private double _value;

        private AverageRating(double value, int numRatings)
        {
            Value = value;
            NumRatings = numRatings;
        }

        public double? Value { get => NumRatings > 0 ? _value : null; private set => _value = value!.Value; }
        public int NumRatings { get; private set; }

        public static AverageRating CreateNew(double rating = 0, int numRatings = 0)
        {
            return new AverageRating(rating, numRatings);
        }

        public void AddNewRating(Rating rating)
        {
            Value = ((Value * NumRatings) + rating.Value) / ++NumRatings;
        }

        public void RemoveRating(Rating rating)
        {
            Value = ((Value * NumRatings) - rating.Value) / --NumRatings;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

#pragma warning disable CS8618
        private AverageRating()
        {
        }
#pragma warning restore CS8618
    }

}