using EnergyPortalApi.Domain.Models.MeterReading;

namespace EnergyPortalApi.Domain.Interfaces.Repositories;

public interface IMeterReadingsRepository
{
    Task<int> AddBulkMeterReadingsAsync(IEnumerable<MeterReading> meterReadings);

    Task<IEnumerable<MeterReading>> GetMeterReadingsForAccountAsync(int accountId);

    Task RemoveAllReadingsAsync();
}
