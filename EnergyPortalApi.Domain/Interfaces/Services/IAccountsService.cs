using EnergyPortalApi.Domain.Models.Accounts;

namespace EnergyPortalApi.Domain.Interfaces.Services;

public interface IAccountsService
{
    Task<IEnumerable<Account>> GetActiveAccountsAsync(IEnumerable<int> accountIds);

    Task<Account> GetAccountByIdAsync(int accountId);
}