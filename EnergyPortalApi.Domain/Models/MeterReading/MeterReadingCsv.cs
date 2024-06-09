namespace EnergyPortalApi.Domain.Models.MeterReading;

public class MeterReadingCsv(string accountId, string meterReadValue, string meterReadingDateTime)
{

    public string MeterReadValue { get; set; } = meterReadValue;

    public string MeterReadingDateTime { get; set; } = meterReadingDateTime;

    public string AccountId { get; set; } = accountId;
}
