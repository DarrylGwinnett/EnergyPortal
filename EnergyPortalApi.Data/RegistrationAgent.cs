using EnergyPortalApi.Data.Context;
using EnergyPortalApi.Data.Repositories;
using EnergyPortalApi.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnergyPortalApi.Data;

public static class RegistrationAgent
{
    public static IServiceCollection RegisterDataServices(this IServiceCollection collection)
    {
        collection.AddDbContext<AccountContext>(options =>
            options.UseSqlServer("name=ConnectionStrings:EnergyPortal"));
        collection.AddDbContext<MeterReadingsContext>(options =>
            options.UseSqlServer("name=ConnectionStrings:EnergyPortal"));

        collection.AddScoped<IAccountsRepository, AccountsRepository>();
        collection.AddScoped<IMeterReadingsRepository, MeterReadingsRepository>();
        return collection;
    }
}
