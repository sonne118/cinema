using Application.Common.Interfaces.Services;

namespace Infrastructure.Persistence.Repositories;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}