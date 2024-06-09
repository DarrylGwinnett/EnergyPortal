using EnergyPortalApi.Data.Context;
using EnergyPortalApi.Data.Transformations;
using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace EnergyPortalApi.Data.Repositories;

internal class AccountsRepository(AccountContext context) : IAccountsRepository
{
    public async Task<IEnumerable<Account>> GetAccountsAsync(IEnumerable<int> accountIds)
    {
        var accounts = await context.Accounts.Where(x => accountIds.Contains(x.Id)).ToListAsync();
        return accounts.Select(x => x.Transform());
    }

    public async Task<Account> GetAccountAsync(int accountId)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        if (account == null)
        {
            throw new AccountNotFoundException($"Account with ID {accountId} was not found.");
        }
        return account.Transform();

    }
}
