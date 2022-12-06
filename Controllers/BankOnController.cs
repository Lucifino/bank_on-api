using Microsoft.AspNetCore.Mvc;
using bank_on_api.Adapters;
using bank_on_api.Models.Entities.BankOn;
using static bank_on_api.Helpers.FileContentResultTypeAttribute;
using bank_on_api.Models.Classes.BankOn;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using System.Net;

namespace bank_on_api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BankOnController : ControllerBase
{
    private readonly BankOnAdapter _bankonAdapter;
    private readonly IConfiguration _configuration;
    private readonly BankOn _db;

    public BankOnController(BankOnAdapter bankonAdapter, IConfiguration configuration, BankOn db)
    {
        _bankonAdapter = bankonAdapter;
        _configuration = configuration;
        _db = db;

    }


    [HttpPost]
    [EnableCors("LoosePolicy")]
    [ProducesResponseType(typeof(Uri), (int)HttpStatusCode.Redirect)]
    public async Task<IActionResult> SaveFinanceRequestAndRedirect([FromBody] FinanceRequestProxy Request)
    {
        try
        {
            return new RedirectResult(await _bankonAdapter.SaveFinanceRequestAndRedirect(Request), true);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}