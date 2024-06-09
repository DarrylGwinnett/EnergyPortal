namespace EnergyPortalApi.Domain.Models.MeterReading
{
    public class MeterReadingSequenceValidationResult(bool success, List<MeterReadingValidationResult> validationResults)
    {
        public bool Success { get; set; } = success;

        public List<MeterReadingValidationResult> ValidationResults { get; set; } = validationResults;
    }
}