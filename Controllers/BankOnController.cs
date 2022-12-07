using Microsoft.AspNetCore.Mvc;
using bank_on_api.Adapters;
using bank_on_api.Models.Entities.BankOn;
using static bank_on_api.Helpers.FileContentResultTypeAttribute;
using bank_on_api.Models.Classes.BankOn;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using System.Net;
using bank_on_api.Entites.Responses;

namespace bank_on_api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
// [EnableCors("LoosePolicy")]
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
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<String> SaveFinanceRequestAndRedirect([FromBody] FinanceRequestProxy Request)
    {
        try
        {
            return await _bankonAdapter.SaveFinanceRequestAndRedirect(Request);
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}