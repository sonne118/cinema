using Application.Common.Interfaces.Services;

namespace ApiApplication.Infrastructure.Persistence.Repositories;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}