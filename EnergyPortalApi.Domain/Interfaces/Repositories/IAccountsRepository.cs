using EnergyPortalApi.Domain.Models.Accounts;

namespace EnergyPortalApi.Domain.Interfaces.Repositories;

public interface IAccountsRepository
{
    Task<IEnumerable<Account>> GetAccountsAsync(IEnumerable<int> accountIds);

    Task<Account> GetAccountAsync(int accountId);
}
