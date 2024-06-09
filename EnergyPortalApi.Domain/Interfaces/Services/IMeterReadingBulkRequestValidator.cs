using EnergyPortalApi.Domain.Models.MeterReading;

namespace EnergyPortalApi.Domain.Interfaces.Services;

public interface IMeterReadingBulkRequestValidator
{
    Task<List<MeterReadingValidationResult>> ValidateMeterReadingBulkRequestAsync(IEnumerable<MeterReading> meterReadingCollection);
}