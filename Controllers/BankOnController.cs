using Microsoft.AspNetCore.Mvc;
using bank_on_api.Adapters;
using bank_on_api.Models.Entities.BankOn;

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
}