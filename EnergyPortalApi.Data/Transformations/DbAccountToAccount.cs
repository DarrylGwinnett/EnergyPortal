using EnergyPortalApi.Data.Models;
using EnergyPortalApi.Domain.Models.Accounts;

namespace EnergyPortalApi.Data.Transformations;

internal static class DbAccountToAccount
{
    internal static Account Transform(this DbAccount input)
    {
        return new(input.Id, input.FirstName, input.LastName);
    }
}

