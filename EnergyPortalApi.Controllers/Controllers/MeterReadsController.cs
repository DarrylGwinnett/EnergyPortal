using EnergyPortalApi.Domain.Interfaces.Repositories;
using EnergyPortalApi.Domain.Interfaces.Services;
using EnergyPortalApi.Domain.Models.CsvParsing;
using EnergyPortalApi.Domain.Models.MeterReading;
using Microsoft.AspNetCore.Mvc;

namespace EnergyPortalApi.Controllers;

[Route("api/meter-reading-uploads")]
[ApiController]
public class MeterReadsController(IMeterReadingsBulkProcessor meterReadsService, IMeterReadingsRepository meterReadingsRepository) : ControllerBase
{
    private readonly IMeterReadingsBulkProcessor meterReadsService = meterReadsService;
    //Would probably not want to shortcut the service layer in most instances, but this is just for the cheat meterread cleardown.
    private readonly IMeterReadingsRepository meterReadingsRepository = meterReadingsRepository;

    /// <summary>
    /// Upload a cscv file containing meter readings
    /// </summary>
    /// <param name="meterReadingFile"></param>
    /// <returns></returns>    
    [HttpPost]
    public async Task<ActionResult<MeterReadingBatchResult>> PostAsync(IFormFile meterReadingFile)
    {
        try
        {
            var result = await meterReadsService.ProcessMeterReadingsAsync(meterReadingFile);
            return Ok(result);
        }
        catch (Exception ex) when (ex is CsvParsingException)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Remove test data
    /// </summary>
    /// <returns></returns>    
    [HttpDelete]
    public async Task<IActionResult> RemoveTestReadingsAsync()
    {
        //Obviously a terrible thing to do for anything but a convenient cleardown for a fake app :)
        await meterReadingsRepository.RemoveAllReadingsAsync();
        return NoContent();

    }

}
