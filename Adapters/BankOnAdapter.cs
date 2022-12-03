using bank_on_api.Models.Entities.BankOn;
using bank_on_api.Helpers;

namespace bank_on_api.Adapters;

public class BankOnAdapter
{
    private readonly BankOn db;
    private readonly IClockService _clockService;
    //private readonly GW gwdb;

    public BankOnAdapter(BankOn _bankon, IClockService clockService)
    {
        db = _bankon;
        _clockService = clockService;
        //gwdb = _gw;
    }
}