using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.Accounts;
using Microsoft.Extensions.Logging;

namespace EnergyPortalApi.Domain.Services;

public class AccountsService(IAccountsRepository accountsRepository, ILogger<AccountsService> logger) : IAccountsService
{
    private readonly IAccountsRepository accountsRepository = accountsRepository;
    private readonly ILogger<AccountsService> logger = logger;

    public async Task<IEnumerable<Account>> GetActiveAccountsAsync(IEnumerable<int> accountIds)
    {
        var accounts = await accountsRepository.GetAccountsAsync(accountIds);
        return accounts;
    }


    public async Task<Account> GetAccountByIdAsync(int accountId)
    {

        return await accountsRepository.GetAccountAsync(accountId);
    }
}
