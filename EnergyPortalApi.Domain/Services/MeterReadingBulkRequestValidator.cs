using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.Accounts;
using EnergyPortalApi.Domain.Models.MeterReading;

namespace EnergyPortalApi.Domain.Services
{
    public class MeterReadingBulkRequestValidator(IMeterReadingsRepository meterReadsRepository, IAccountsService accountsService, IDateTimeProvider dateTimeProvider) : IMeterReadingBulkRequestValidator
    {
        private readonly IMeterReadingsRepository meterReadsRepository = meterReadsRepository;
        private readonly IAccountsService accountsService = accountsService;
        private readonly IDateTimeProvider dateTimeProvider = dateTimeProvider;

        public async Task<List<MeterReadingValidationResult>> ValidateMeterReadingBulkRequestAsync(IEnumerable<MeterReading> meterReadingCollection)
        {
            var accountIds = meterReadingCollection.Select(x => x.AccountId).Distinct();
            var validationResults = new List<MeterReadingValidationResult>();

            foreach (var accountId in accountIds)
            {
                var readingsForAccount = meterReadingCollection.Where(reading => reading.AccountId == accountId);
                validationResults.AddRange(await ValidateReadingsForAccount(accountId, readingsForAccount));

            }
            return validationResults;
        }



        /// <summary>
        /// Ensures that if multiple readings are provided for an account in a batch upload:
        /// * meter reads are not lower than any recorded previously.
        /// * meter read datetime is unique
        /// </summary>
        /// <param name="newMeterReadings"></param>
        /// <returns>Bool representing if new account meter reading collection is valid</returns>
        internal MeterReadingSequenceValidationResult ValidateAccountMeterReadingSequence(IEnumerable<MeterReading> newMeterReadings)
        {
            //If there are multiple readings for the same account and the same time, there's something wrong
            // We could potentially just remove x-1 if there are complete duplicates, but what do we do if the times match but not the value?
            // Rejecting the group and forcing the user to review and resubmit a valid collection seems simplest/safest
            var distinctReadingByTime = newMeterReadings.DistinctBy(x => x.MeterReadingDateTime);
            if (newMeterReadings.Count() != distinctReadingByTime.Count())
            {
                return new(false, newMeterReadings.Select(mr => new MeterReadingValidationResult(false, mr, FailureCategoryType.InvalidSequenceForAccount)).ToList());
            };

            var sortedReadings = newMeterReadings.OrderBy(mr => mr.MeterReadingDateTime).ToList();
            int highestReadValueInSequence = 0;
            for (int i = 0; i < sortedReadings.Count; i++)
            {
                if (sortedReadings[i].MeterReadValue < highestReadValueInSequence)
                {
                    return new(false, newMeterReadings.Select(mr => new MeterReadingValidationResult(false, mr, FailureCategoryType.InvalidSequenceForAccount)).ToList());
                }
                highestReadValueInSequence = sortedReadings[i].MeterReadValue;
            }

            return new(true, newMeterReadings.Select(mr => new MeterReadingValidationResult(true, mr)).ToList());
        }

        private async Task<IEnumerable<MeterReadingValidationResult>> ValidateReadingsForAccount(int accountId, IEnumerable<MeterReading> meterReadingCollection)
        {
            try
            {
                var accountDetails = await accountsService.GetAccountByIdAsync(accountId);
                var accountReadingResults = new List<MeterReading>();

                var sequenceValidationResult = ValidateAccountMeterReadingSequence(meterReadingCollection);
                if (sequenceValidationResult.Success == false)
                {
                    return meterReadingCollection.Select(reading =>
                    {
                        return new MeterReadingValidationResult(false, reading, FailureCategoryType.InvalidSequenceForAccount);
                    });
                }

                return await ValidateReadingsFromValidAccountSequences(accountId, meterReadingCollection);
            }
            catch (AccountNotFoundException)
            {
                return meterReadingCollection.Select(reading =>
                {
                    return new MeterReadingValidationResult(false, reading, FailureCategoryType.InvalidAccountId);
                });
            }
        }

        //On further consideration, I'd split this out into an IAccountMeterReadingsValidator separate from the bulk validator, but I'm running out of time :(
        private async Task<List<MeterReadingValidationResult>> ValidateReadingsFromValidAccountSequences(int accountId, IEnumerable<MeterReading> newReadingsForAccount)
        {
            var validationResults = new List<MeterReadingValidationResult>();
            foreach (var newMeterReading in newReadingsForAccount)
            {
                var readingValidationResult = await ValidateMeterReadingAsync(newMeterReading);
                if (readingValidationResult.Success)
                {
                    validationResults.Add(new MeterReadingValidationResult(true, newMeterReading));
                }
                else
                {
                    validationResults.Add(readingValidationResult);
                }
            }
            return validationResults;
        }

        internal async Task<MeterReadingValidationResult> ValidateMeterReadingAsync(MeterReading newMeterReading)
        {
            if (ReadingOutOfRange(newMeterReading)) { return new(false, newMeterReading, FailureCategoryType.MeterReadingOutOfRange); }
            if (ReadingInFuture(newMeterReading)) { return new(false, newMeterReading, FailureCategoryType.ReadingDateIsInFuture); }
            // If we have multiple readings on an account which are in range and have a past date, we may hit the DB multiple times.
            // If we split this out into an IAccountMeterReadingsValidator, it will want some sort of caching mechanism
            var existingReadings = await meterReadsRepository.GetMeterReadingsForAccountAsync(newMeterReading.AccountId);
            if (MeterReadingIsDuplicate(newMeterReading, existingReadings)) { return new(false, newMeterReading, FailureCategoryType.DuplicateReading); }
            if (NewerReadingsExist(newMeterReading, existingReadings)) { return new(false, newMeterReading, FailureCategoryType.NewerReadingExists); }
            if (HigherReadingsExist(newMeterReading, existingReadings)) { return new(false, newMeterReading, FailureCategoryType.HigherReadingExists); }
            return new(true, newMeterReading);
        }


        private bool ReadingInFuture(MeterReading meterReading)
        {
            return dateTimeProvider.UtcNow() < meterReading.MeterReadingDateTime;
        }

        private static bool ReadingOutOfRange(MeterReading meterReading)
        {
            //TODO: values currently only used in one place but would ideally be driven by config.
            return meterReading.MeterReadValue < 0 || meterReading.MeterReadValue > 99999;
        }

        private static bool HigherReadingsExist(MeterReading meterReading, IEnumerable<MeterReading> meterReadsForThisAccount)
        {
            var highestExistingReading = meterReadsForThisAccount.OrderBy(m => m.MeterReadValue).LastOrDefault();
            return highestExistingReading != null && meterReading.MeterReadValue < highestExistingReading.MeterReadValue;
        }

        private static bool NewerReadingsExist(MeterReading meterReading, IEnumerable<MeterReading> meterReadsForThisAccount)
        {
            var highestExistingReading = meterReadsForThisAccount.OrderBy(m => m.MeterReadingDateTime).LastOrDefault();
            return highestExistingReading != null && meterReading.MeterReadingDateTime < highestExistingReading.MeterReadingDateTime;
        }

        private static bool MeterReadingIsDuplicate(MeterReading meterReading, IEnumerable<MeterReading> meterReadsForThisAccount)
        {
            return meterReadsForThisAccount.Any(m => m.MeterReadValue == meterReading.MeterReadValue && m.MeterReadingDateTime == meterReading.MeterReadingDateTime);
        }
    }
}
