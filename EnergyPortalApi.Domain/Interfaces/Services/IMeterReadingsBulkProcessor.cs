using EnergyPortalApi.Domain.Models.MeterReading;
using Microsoft.AspNetCore.Http;

namespace EnergyPortalApi.Domain.Interfaces.Services;

public interface IMeterReadingsBulkProcessor
{
    Task<MeterReadingBatchResult> ProcessMeterReadingsAsync(IFormFile meterReads);
}
