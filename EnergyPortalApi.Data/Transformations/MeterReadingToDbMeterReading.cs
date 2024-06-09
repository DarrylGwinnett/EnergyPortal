using EnergyPortalApi.Data.Models;
using EnergyPortalApi.Domain.Models.MeterReading;

namespace EnergyPortalApi.Data.Transformations;

internal static class MeterReadingsToDbMeterReadings
{
    public static DbMeterReading Transform(this MeterReading input)
    {
        return new DbMeterReading
        {
            AccountId = input.AccountId,
            MeterReadValue = input.MeterReadValue,
            MeterReadingDateTime = input.MeterReadingDateTime
        };
    }
}
