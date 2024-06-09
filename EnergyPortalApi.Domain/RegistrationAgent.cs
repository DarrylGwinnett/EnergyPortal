using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EnergyPortalApi.Domain;


public static class RegistrationAgent
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection collection)
    {
        collection.AddScoped<IDateTimeProvider, DateTimeProvider>();
        collection.AddScoped<IAccountsService, AccountsService>();
        collection.AddScoped<IMeterReadingsBulkProcessor, MeterReadingsBulkProcessor>();
        collection.AddScoped<IMeterReadingBulkRequestValidator, MeterReadingBulkRequestValidator>();
        collection.AddScoped<ICsvService, CsvService>();
        return collection;
    }
}

