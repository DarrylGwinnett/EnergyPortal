namespace EnergyPortalApi.Domain.Models.MeterReading;

public class MeterReading(int accountId, int meterReadValue, DateTime meterReadingDateTime)
{
    public int MeterReadValue { get; set; } = meterReadValue;

    public DateTime MeterReadingDateTime { get; set; } = meterReadingDateTime;

    public int AccountId { get; set; } = accountId;

}
