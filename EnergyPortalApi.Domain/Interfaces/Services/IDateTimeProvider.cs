namespace EnergyPortalApi.Domain.Interfaces.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow();
}