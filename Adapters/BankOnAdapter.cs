using bank_on_api.Models.Entities.BankOn;
using bank_on_api.Helpers;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using bank_on_api.Models.Classes.BankOn;
using System.Globalization;

namespace bank_on_api.Adapters;

public class BankOnAdapter
{
    private readonly BankOn db;
    private readonly IClockService _clockService;


    public BankOnAdapter(BankOn _bankon, IClockService clockService)
    {
        db = _bankon;
        _clockService = clockService;
    }

    public async Task<String> SaveFinanceRequestAndRedirect(FinanceRequestProxy request)
    {

        // var transaction = await db.Database.BeginTransactionAsync(_cancellationToken);

        try
        {
            Console.WriteLine(request);

            var new_reference_no = $"{request.FirstName}-{request.LastName}-{request.DateOfBirth.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

            var savedRequest = db.FinanceRequest.FirstOrDefault(x => x.ReferenceNo == new_reference_no);

            var initial_status = db.FinanceRequestStatus.FirstOrDefault(x => x._case == 0);

            var idParam = "";

            if (savedRequest is not null)
            {
                idParam = savedRequest.FinanceRequestId.ToString();
            }
            else
            {
                var financeRequest = await db.FinanceRequest.AddAsync(
                    new FinanceRequest()
                    {
                        AmountRequired = request.AmountRequired,
                        Term = request.Term,
                        Title = request.Title,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        DateOfBirth = request.DateOfBirth,
                        Mobile = request.Mobile,
                        Email = request.Email,
                        ReferenceNo = new_reference_no,
                        DateOfApplication = _clockService.LocalNow.Date,
                        FinanceRequestStatusId = initial_status?.FinanceRequestStatusId

                    }
                );

                financeRequest.Entity.FinanceRequestLog.Add(
                    new FinanceRequestLog
                    {
                        Title = "Creation",
                        Description = "Request",
                        Content = $"{financeRequest.Entity.FirstName} {financeRequest.Entity.LastName} has applied for a financing worth ${financeRequest.Entity.AmountRequired}. Reference No ${financeRequest.Entity.ReferenceNo}",
                        DateCreated = _clockService.Now,
                    }
                );



                await db.SaveChangesAsync();

                var addedRequest = db.FinanceRequest.FirstOrDefault(x => x.ReferenceNo == new_reference_no);

                idParam = addedRequest?.FinanceRequestId.ToString();
                // await transaction.CommitAsync();
            }

            return $"http://localhost:4200/admin/calculate/{idParam}";
            //return $"https://kind-sand-099505e10.2.azurestaticapps.net/admin/calculate/{idParam}";
        }
        catch (Exception e)
        {
            // await transaction.RollbackAsync(_cancellationToken);
            throw e;
        }

    }
}