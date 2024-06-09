using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.Accounts;
using EnergyPortalApi.Domain.Models.CsvParsing;
using EnergyPortalApi.Domain.Models.MeterReading;
using EnergyPortalApi.Domain.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;

namespace EnergyPortalApi.Domain.Tests.MeterReadingsBulkProcessorTests
{
    [TestFixture]
    public class MeterReadingsBulkProcessorTests
    {
        private IMeterReadingsRepository meterReadingsRepository;
        private IMeterReadingBulkRequestValidator validator;
        private ICsvService csvService;
        private ILogger<MeterReadingsBulkProcessor> logger;
        private readonly FormFile mockInput = new FormFile(new MemoryStream(), new(), new(), "", "");


        [SetUp]
        public void Setup()
        {
            meterReadingsRepository = A.Fake<IMeterReadingsRepository>();
            csvService = A.Fake<ICsvService>();
            validator = A.Fake<IMeterReadingBulkRequestValidator>();
            logger = A.Fake<Logger<MeterReadingsBulkProcessor>>();
        }

        [Test]
        public async Task GivenAParseFailure_ThenCorrectParseCountAndDataReturned()
        {
            var failedRows = new List<FailedRecord>() { new("failed", "data") };
            var reading = new MeterReading(234, 1, new DateTime(2023, 12, 31, 12, 00, 00));
            var readingList = new List<MeterReading>()
            {
              reading
             };
            var csvResult = new CsvParseResult<MeterReading>(0, 1, readingList, failedRows);
            A.CallTo(() => csvService.ParseFromCsv<MeterReading>(A<IFormFile>._)).Returns(csvResult);
            A.CallTo(() => validator.ValidateMeterReadingBulkRequestAsync(A<IEnumerable<MeterReading>>._)).Returns(new List<MeterReadingValidationResult>());
            var sut = new MeterReadingsBulkProcessor(meterReadingsRepository, csvService, validator, logger);

            var result = await sut.ProcessMeterReadingsAsync(mockInput);
            Assert.Multiple(() =>
                {
                    Assert.That(result.FailedReadingsCount, Is.EqualTo(1));
                    Assert.That(result.SuccessfulReadingsCount, Is.EqualTo(0));
                    Assert.That(result.CsvParseFailures.Count, Is.EqualTo(1));
                    Assert.That(result.CsvParseFailures.First().RawRecord, Is.EqualTo("data"));
                    Assert.That(result.CsvParseFailures.First().Message, Is.EqualTo("failed"));
                });
            A.CallTo(() => meterReadingsRepository.AddBulkMeterReadingsAsync(A<List<MeterReading>>._))
               .MustNotHaveHappened();
        }

        [Test]
        public async Task GivenAValidationFailure_ThenCorrectCountAndDataReturned()
        {
            var failedCsvRows = new List<FailedRecord>() { };
            var reading = new MeterReading(234, 1, new DateTime(2023, 12, 31, 12, 00, 00));
            var readingList = new List<MeterReading>()
            {
              reading
             };
            var csvResult = new CsvParseResult<MeterReading>(1, 0, readingList, failedCsvRows);
            var mockValidationResult = new List<MeterReadingValidationResult>() {
                new MeterReadingValidationResult(false, reading, FailureCategoryType.MeterReadingOutOfRange)  };
            A.CallTo(() => csvService.ParseFromCsv<MeterReading>(A<IFormFile>._)).Returns(csvResult);
            A.CallTo(() => validator.ValidateMeterReadingBulkRequestAsync(A<IEnumerable<MeterReading>>._)).Returns(mockValidationResult);
            var sut = new MeterReadingsBulkProcessor(meterReadingsRepository, csvService, validator, logger);

            var result = await sut.ProcessMeterReadingsAsync(mockInput);
            Assert.Multiple(() =>
            {
                Assert.That(result.FailedReadingsCount, Is.EqualTo(1));
                Assert.That(result.SuccessfulReadingsCount, Is.EqualTo(0));
                Assert.That(result.CsvParseFailures.Count, Is.EqualTo(0));
                Assert.That(result.ValidationFailures.Count, Is.EqualTo(1));
                Assert.That(result.ValidationFailures.First().Success, Is.False);
                Assert.That(result.ValidationFailures.First().FailureCategory, Is.EqualTo(FailureCategoryType.MeterReadingOutOfRange));
                Assert.That(result.ValidationFailures.First().MeterReading!.MeterReadValue, Is.EqualTo(reading.MeterReadValue));
            });
            A.CallTo(() => meterReadingsRepository.AddBulkMeterReadingsAsync(A<List<MeterReading>>._))
               .MustNotHaveHappened();
        }

        [Test]
        public async Task GivenASuccessfullyValidatedEntry_ThenCorrectCountsReturned()
        {
            var failedRows = new List<FailedRecord>();
            var reading = new MeterReading(234, 1, new DateTime(2023, 12, 31, 12, 00, 00));
            var readingList = new List<MeterReading>()
            {
              reading
             };
            var mockValidationResult = new List<MeterReadingValidationResult>() {
                new MeterReadingValidationResult(true, reading)  };
            var csvResult = new CsvParseResult<MeterReading>(1, 0, readingList, failedRows);
            A.CallTo(() => csvService.ParseFromCsv<MeterReading>(A<IFormFile>._)).Returns(csvResult);
            A.CallTo(() => validator.ValidateMeterReadingBulkRequestAsync(A<IEnumerable<MeterReading>>._)).Returns(mockValidationResult);
            var sut = new MeterReadingsBulkProcessor(meterReadingsRepository, csvService, validator, logger);

            var result = await sut.ProcessMeterReadingsAsync(mockInput);
            Assert.Multiple(() =>
            {
                Assert.That(result.FailedReadingsCount, Is.EqualTo(0));
                Assert.That(result.SuccessfulReadingsCount, Is.EqualTo(1));
                Assert.That(result.CsvParseFailures.Count, Is.EqualTo(0));
            });
            A.CallTo(() => meterReadingsRepository.AddBulkMeterReadingsAsync(readingList))
               .MustHaveHappenedOnceExactly();
        }



    }
}