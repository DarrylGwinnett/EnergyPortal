using EnergyPortalApi.Domain.Models.CsvParsing;
using Microsoft.AspNetCore.Http;

namespace EnergyPortalApi.Domain.Interfaces.Services;

public interface ICsvService
{
    CsvParseResult<T> ParseFromCsv<T>(IFormFile files);
}