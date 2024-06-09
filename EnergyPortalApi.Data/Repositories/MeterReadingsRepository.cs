using EnergyPortalApi.Data.Context;
using EnergyPortalApi.Data.Transformations;
using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Models.MeterReading;
using Microsoft.EntityFrameworkCore;

namespace EnergyPortalApi.Data.Repositories;

internal class MeterReadingsRepository(MeterReadingsContext meterReadingsContext) : IMeterReadingsRepository
{
    private readonly MeterReadingsContext meterReadingsContext = meterReadingsContext;

    public Task<int> AddBulkMeterReadingsAsync(IEnumerable<MeterReading> meterReadings)
    {
        var meterReadingEntities = meterReadings.Select(x => x.Transform());
        meterReadingsContext.AddRangeAsync(meterReadingEntities);
        return meterReadingsContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<MeterReading>> GetMeterReadingsForAccountAsync(int accountId)
    {
        var entities = await meterReadingsContext.MeterReadings.Where(x => x.AccountId == accountId).ToListAsync();
        return entities.Select(x => x.Transform());
    }

    public async Task RemoveAllReadingsAsync()
    {
        await meterReadingsContext.MeterReadings.ExecuteDeleteAsync();
    }
}
