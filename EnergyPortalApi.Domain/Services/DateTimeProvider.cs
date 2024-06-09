using EnergyPortalApi.Domain.Interfaces.Services;

namespace EnergyPortalApi.Domain.Services;

public class DateTimeProvider : IDateTimeProvider
{

    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}