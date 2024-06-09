using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.Accounts;
using EnergyPortalApi.Domain.Models.MeterReading;
using EnergyPortalApi.Domain.Services;
using FakeItEasy;

namespace EnergyPortalApi.Domain.Tests.AccountMeterReadingsValidatorTests
{
    internal class ValidateMeterReadingAsync_Tests
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
        public async Task GivenReadingWithReadingValueLowerThanExisting_ThenMeterReadingIsRejected()
        {

            var accountId = 123;
            var accountInput = new[] { accountId };
            var accountResponse = new[] { new Account(accountId, "first", "last") };
            var inputData = new MeterReading(accountId, 1, DateTime.UtcNow);
            var repoData = new List<MeterReading>()
            {
                new(accountId, 2, DateTime.UtcNow.AddDays(-1))
            };
            A.CallTo(() => meterReadingsRepository.GetMeterReadingsForAccountAsync(accountId)).Returns(repoData);
            A.CallTo(() => dateTimeProvider.UtcNow()).Returns(new DateTime(2024, 12, 31, 12, 00, 00));
            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.FailureCategory, Is.EqualTo(FailureCategoryType.HigherReadingExists));
            });
        }


        [Test]
        public async Task GivenReadingWithReadingDateInFuture_ThenMeterReadingIsRejected()
        {

            var accountId = 123;
            var accountInput = new[] { accountId };
            var accountResponse = new[] { new Account(accountId, "first", "last") };
            var inputData = new MeterReading(accountId, 1, new DateTime(2023, 12, 31, 12, 00, 00));
            var repoData = new List<MeterReading>()
            {
                new(accountId, 2, DateTime.UtcNow.AddDays(-1))
            };
            A.CallTo(() => meterReadingsRepository.GetMeterReadingsForAccountAsync(accountId)).Returns(repoData);
            A.CallTo(() => dateTimeProvider.UtcNow()).Returns(new DateTime(2023, 12, 30, 12, 00, 00));
            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.FailureCategory, Is.EqualTo(FailureCategoryType.ReadingDateIsInFuture));
            });
        }

        [Test]
        public async Task GivenReadingWithReadingDateOlderThanExisting_ThenMeterReadingIsRejected()
        {

            var accountId = 123;
            var earlierDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var laterDate = new DateTime(2023, 12, 31, 12, 59, 00);
            var inputData = new MeterReading(accountId, 3, earlierDate);
            var repoData = new List<MeterReading>()
            {
                new(accountId, 2, laterDate)
            };
            A.CallTo(() => meterReadingsRepository.GetMeterReadingsForAccountAsync(accountId)).Returns(repoData);
            A.CallTo(() => dateTimeProvider.UtcNow()).Returns(new DateTime(2024, 12, 31, 12, 00, 00));

            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.FailureCategory, Is.EqualTo(FailureCategoryType.NewerReadingExists));
            });
        }


        [Test]
        public async Task GivenReadingWithReadingDateAndValueThatExists_ThenMeterReadingIsRejected()
        {

            var accountId = 123;
            var readingValue = 1;
            var readingDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var inputData = new MeterReading(accountId, readingValue, readingDate);
            var repoData = new List<MeterReading>()
            {
                new(accountId, readingValue, readingDate)
            };
            A.CallTo(() => meterReadingsRepository.GetMeterReadingsForAccountAsync(accountId)).Returns(repoData);
            A.CallTo(() => dateTimeProvider.UtcNow()).Returns(new DateTime(2024, 12, 31, 12, 00, 00));
            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.FailureCategory, Is.EqualTo(FailureCategoryType.DuplicateReading));
            });
        }

        [Test]
        public async Task GivenReadingWithValueOverRange_ThenMeterReadingIsRejected()
        {

            var accountId = 123;
            var readingValue = 100000;
            var readingDate = new DateTime(2023, 12, 31, 12, 00, 00);

            var inputData = new MeterReading(accountId, readingValue, readingDate);
            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.FailureCategory, Is.EqualTo(FailureCategoryType.MeterReadingOutOfRange));
            });
        }


        [Test]
        public async Task GivenReadingWithValueUnderRange_ThenMeterReadingIsRejected()
        {

            var accountId = 123;
            var readingValue = -1;
            var readingDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var inputData = new MeterReading(accountId, readingValue, readingDate);
            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(false));
                Assert.That(result.FailureCategory, Is.EqualTo(FailureCategoryType.MeterReadingOutOfRange));
            });
        }

        [Test]
        public async Task GivenReadingWithValueOnLowerBoundary_ThenMeterReadingIsAccepted()
        {

            var accountId = 123;
            var readingValue = 0;
            var readingDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var inputData = new MeterReading(accountId, readingValue, readingDate);
            A.CallTo(() => dateTimeProvider.UtcNow()).Returns(new DateTime(2024, 12, 31, 12, 00, 00));

            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);

            Assert.That(result.Success, Is.EqualTo(true));
        }


        [Test]
        public async Task GivenReadingWithValueOnUpperBoundary_ThenMeterReadingIsAccepted()
        {

            var accountId = 123;
            var readingValue = 0;
            var readingDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var inputData = new MeterReading(accountId, readingValue, readingDate);
            A.CallTo(() => dateTimeProvider.UtcNow()).Returns(new DateTime(2024, 12, 31, 12, 00, 00));
            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);
            Assert.That(result.Success, Is.EqualTo(true));
        }

        [Test]
        public async Task GivenReadingWithHigherValueAndNewerDate_ThenMeterReadingIsSuccessful()
        {
            var accountId = 123;
            var earlierDate = new DateTime(2023, 12, 31, 12, 00, 00);
            var laterDate = new DateTime(2023, 12, 31, 12, 59, 00);
            var inputData = new MeterReading(accountId, 3, laterDate);

            var repoData = new List<MeterReading>()
            {
                new(accountId, 2, earlierDate)
            };
            A.CallTo(() => meterReadingsRepository.GetMeterReadingsForAccountAsync(accountId)).Returns(repoData);
            A.CallTo(() => dateTimeProvider.UtcNow()).Returns(new DateTime(2024, 12, 31, 12, 00, 00));
            var sut = new MeterReadingBulkRequestValidator(meterReadingsRepository, accountsService, dateTimeProvider);

            var result = await sut.ValidateMeterReadingAsync(inputData);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.EqualTo(true));
                Assert.That(result.FailureCategory, Is.EqualTo(FailureCategoryType.None));
            });
        }
    }
}
