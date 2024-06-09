using CsvHelper;
using CsvHelper.Configuration;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.CsvParsing;
using EnergyPortalApi.Domain.Models.MeterReading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace EnergyPortalApi.Domain.Services;

public class CsvService(ILogger<CsvService> logger) : ICsvService
{
    private readonly ILogger<CsvService> logger = logger;

    public CsvParseResult<T> ParseFromCsv<T>(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".csv")
        {
            throw new CsvParsingException($"File extension must be .csv but was {extension}");
        }
        var failedRecords = new List<FailedRecord>();
        var goodResults = new List<T>();
        using var reader = new StreamReader(file.OpenReadStream());
        var csvConfig = new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
        {
            PrepareHeaderForMatch = args => args.Header.ToLowerInvariant()
        };
        var csvReader = new CsvReader(reader, csvConfig);
        while (csvReader.Read())
        {
            try
            {
                var record = csvReader.GetRecord<T>();
                goodResults.Add(record);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception parsing Csv input: {Message}", ex.Message);
                failedRecords.Add(new(ex.Message, csvReader.Context.Parser.RawRecord));
            }
        }

        if (goodResults.Count == 0) { throw new CsvParsingException($"No records could be parsed from the provided file."); }
        return new(goodResults.Count,
            failedRecords.Count,
            goodResults,
            failedRecords);
    }
}


