using Cinema.Application.Common.Interfaces.Services;

namespace Cinema.Infrastructure.Services;




public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
