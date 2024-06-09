using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace EnergyPortalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountsService accountsService) : ControllerBase
{
    private readonly IAccountsService accountsService = accountsService;

    /// <summary>
    /// Get account by its ID.
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Account>> GetAccountAsync(int accountId)
    {
        try
        {
            return Ok(await accountsService.GetAccountByIdAsync(accountId));
        }
        catch (Exception ex) when (ex is AccountNotFoundException)
        {
            return NotFound();
        }
    }

}
