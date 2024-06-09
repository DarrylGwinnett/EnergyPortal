using System.Text.Json.Serialization;

namespace EnergyPortalApi.Domain.Models.MeterReading;

public  class MeterReadingValidationResult
{
    public bool Success { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FailureCategoryType FailureCategory { get; set; }

    public MeterReading? MeterReading { get; set; }

    public MeterReadingValidationResult() { }

    public MeterReadingValidationResult(bool success, MeterReading meterReading)
    {
        Success = success;
        MeterReading = meterReading;
        FailureCategory = FailureCategoryType.None;
    }

    public MeterReadingValidationResult(bool success, MeterReading meterReading, FailureCategoryType failureCategory)
    {
        Success = success;
        FailureCategory = failureCategory;
        MeterReading = meterReading;
    }
}