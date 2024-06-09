namespace EnergyPortalApi.Domain.Models.MeterReading;

public class MeterReadingBatchResult
{
    public int SuccessfulReadingsCount { get; set; }

    public int FailedReadingsCount { get; set; }

    public List<FailedRecord> CsvParseFailures { get; set; } = new();

    public List<MeterReadingValidationResult> ValidationFailures { get; set; } = new();

    public MeterReadingBatchResult(int successfulReadingsCount, int failedReadingsCount, List<FailedRecord> csvParseFailures, List<MeterReadingValidationResult> failuredValidations)
    {
        SuccessfulReadingsCount = successfulReadingsCount;
        FailedReadingsCount = failedReadingsCount;
        CsvParseFailures = csvParseFailures;
        ValidationFailures = failuredValidations;
    }

    public MeterReadingBatchResult()
    {
    }
}
