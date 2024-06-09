using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.MeterReading;
using EnergyPortalApi.Domain.Services;
using FakeItEasy;

namespace EnergyPortalApi.Domain.Tests.AccountMeterReadingsValidatorTests
{
    internal class ValidateAccountMeterReadingSequence_Tests
    {

        private IMeterReadingsRepository meterReadingsRepository;
        private IDateTimeProvider dateTimeProvider;
        private IAccountsService accountsService;

        [SetUp]
        public void Setup()
        {
            meterReadingsRepository = A.Fake<IMeterReadingsRepository>();
            dateTimeProvider = A.Fake<IDateTimeProvider>();
            accountsService = A.Fake<IAccountsService>();
        }

        [Test]
        public void GivenSingleRead_ThenSequenceIsValid()
        {
            var accountId = 123;
            var inputData = new List<MeterReading>() {
                 new MeterReading(accountId, 1, DateTime.UtcNow)
             };

            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = sut.ValidateAccountMeterReadingSequence(inputData);
            Assert.That(result.Success, Is.EqualTo(true));
        }

        [Test]
        public void GivenReadsWithDuplicateTimes_ThenSequenceIsRejected()
        {
            var accountId = 123;

            var readingDate = new DateTime(2023, 12, 31, 12, 00, 00);

            var inputData = new List<MeterReading>() {
                 new MeterReading(accountId, 2, readingDate),
                 new MeterReading(accountId, 1, readingDate)
             };


            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = sut.ValidateAccountMeterReadingSequence(inputData);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.ValidationResults.Count, Is.EqualTo(2));
                Assert.That(result.ValidationResults.All(vr => vr.FailureCategory == FailureCategoryType.InvalidSequenceForAccount));
            });
        }


        [Test]
        public void GivenNewerReadWithLowerValue_ThenSequenceIsRejected()
        {
            var accountId = 123;

            var earlierDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var laterDate = new DateTime(2023, 12, 31, 12, 59, 00);
            var inputData = new List<MeterReading>() {
                 new(accountId, 4, earlierDate),
                 new(accountId, 3, laterDate)
             };

            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = sut.ValidateAccountMeterReadingSequence(inputData);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.ValidationResults.Count, Is.EqualTo(2));
                Assert.That(result.ValidationResults.All(vr => vr.FailureCategory == FailureCategoryType.InvalidSequenceForAccount));
            });
        }

        [Test]
        public void GivenNewerReadWithHigherValue_ThenSequenceIsAccepted()
        {
            var accountId = 123;
            var readingDate = new DateTime(2023, 12, 31, 12, 00, 00);

            var earlierDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var laterDate = new DateTime(2023, 12, 31, 12, 59, 00);
            var inputData = new List<MeterReading>() {
                 new MeterReading(accountId, 2, earlierDate),
                 new MeterReading(accountId, 3, laterDate)
             };

            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = sut.ValidateAccountMeterReadingSequence(inputData);
            Assert.That(result.Success, Is.EqualTo(true));
        }
    }
}
