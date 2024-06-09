using EnergyPortalApi.Data.Models;
using EnergyPortalApi.Domain.Models.MeterReading;

namespace EnergyPortalApi.Data.Transformations;

internal static class DbMeterReadingToMeterReading
{
    internal static MeterReading Transform(this DbMeterReading input)
    {
        return new MeterReading(input.AccountId, input.MeterReadValue, input.MeterReadingDateTime);
    }
}