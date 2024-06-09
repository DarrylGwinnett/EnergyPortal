using Microsoft.EntityFrameworkCore;

namespace EnergyPortalApi.Data.Models;

internal class DbMeterReading()
{
    public int Id { get; set; }

    public int MeterReadValue { get; set; }

    public DateTime MeterReadingDateTime { get; set; }

    public int AccountId { get; set; }
}