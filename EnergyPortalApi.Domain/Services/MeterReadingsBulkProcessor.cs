using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.MeterReading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EnergyPortalApi.Domain.Services;

public class MeterReadingsBulkProcessor(IMeterReadingsRepository meterReadsRepository,
    ICsvService csvService,
    IMeterReadingBulkRequestValidator meterReadingValidator,
    ILogger<MeterReadingsBulkProcessor> logger) : IMeterReadingsBulkProcessor
{
    private readonly IMeterReadingsRepository meterReadsRepository = meterReadsRepository;
    private readonly ICsvService csvService = csvService;
    private readonly IMeterReadingBulkRequestValidator meterReadingValidator = meterReadingValidator;
    private readonly ILogger<MeterReadingsBulkProcessor> logger = logger;

    public async Task<MeterReadingBatchResult> ProcessMeterReadingsAsync(IFormFile meterReadsFile)
    {
        var csvParseResult = csvService.ParseFromCsv<MeterReading>(meterReadsFile);
        var validationResults = await ProcessMeterReadingsAsync(csvParseResult.ParsedRecords);
        validationResults.CsvParseFailures = csvParseResult.FailedRecords;
        validationResults.FailedReadingsCount += csvParseResult.FailedRowCount;
        return validationResults;
    }
    public async Task<MeterReadingBatchResult> ProcessMeterReadingsAsync(IEnumerable<MeterReading> meterReadings)
    {
        List<MeterReadingValidationResult> validationResults = await meterReadingValidator.ValidateMeterReadingBulkRequestAsync(meterReadings);
        var successfullResults = validationResults.Where(vr => vr.Success == true);
        if (successfullResults.Any())
        {
            List<MeterReading> readings = successfullResults.Select(vr => vr.MeterReading!).ToList();
            await meterReadsRepository.AddBulkMeterReadingsAsync(readings);
        }
        var failedValidationEntries = validationResults.Where(mr => mr.Success == false).ToList();
        return new()
        {
            SuccessfulReadingsCount = successfullResults.Count(),
            FailedReadingsCount = failedValidationEntries.Count,
            ValidationFailures = failedValidationEntries
        };
    }
}
