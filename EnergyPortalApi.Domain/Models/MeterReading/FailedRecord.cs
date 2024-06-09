namespace EnergyPortalApi.Domain.Models.MeterReading;

public class FailedRecord(string message, string rawRow)
{
    public string Message { get; set; } = message;

    public string RawRecord { get; set; } = rawRow;
}