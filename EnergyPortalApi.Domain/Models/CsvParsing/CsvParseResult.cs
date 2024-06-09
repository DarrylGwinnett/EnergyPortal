using EnergyPortalApi.Domain.Models.MeterReading;

namespace EnergyPortalApi.Domain.Models.CsvParsing;

public class CsvParseResult<T>(int successfulRowCount, int failedRowCount, List<T> parsedRecords, List<FailedRecord> failedRecords)
{
    public int SuccessfulRowCount { get; set; } = successfulRowCount;
    public int FailedRowCount { get; set; } = failedRowCount;
    public List<T> ParsedRecords { get; set; } = parsedRecords;
    public List<FailedRecord> FailedRecords { get; set; } = failedRecords;
}
