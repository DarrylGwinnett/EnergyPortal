namespace EnergyPortalApi.Domain.Models.MeterReading;


public enum FailureCategoryType
{
    None,
    MeterReadingOutOfRange,
    HigherReadingExists,
    NewerReadingExists,
    DuplicateReading,
    InvalidSequenceForAccount,
    ReadingDateIsInFuture,
    InvalidAccountId
}