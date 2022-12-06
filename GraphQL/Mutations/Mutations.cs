using System.Globalization;
using System.Net;
using bank_on_api.Entites.Responses;
using bank_on_api.Helpers;
using bank_on_api.Models.Classes.BankOn;
using bank_on_api.Models.Entities.BankOn;
using EFCore.BulkExtensions;
using NodaTime;
using Serilog;

namespace bank_on_api.GraphQL.Mutations
{
    public partial class Mutation
    {
        private readonly IHttpContextAccessor _accessor;

        public Mutation(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        [UseDbContext(typeof(BankOn))]
        public async Task<Response> AddFinanceRequestAsync(
            FinanceRequestProxy input,
            [ScopedService] BankOn db,
            [Service] IConfiguration configuration,
            [Service] IClockService clockService,
            CancellationToken cancellationToken
            )
        {


            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var user_ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();


            try
            {


                var initial_status = db.FinanceRequestStatus.FirstOrDefault(x => x._case == 0);

                if (initial_status is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        ResponseLabel = "Server Error!",
                        ResponseMessage = "Case 0 for finance request statuses has not been set"
                    };

                }

                var new_reference_no = $"{input.FirstName}-{input.LastName}-{input.DateOfBirth.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";


                var financeRequest = await db.FinanceRequest.AddAsync(
                    new FinanceRequest()
                    {
                        AmountRequired = input.AmountRequired,
                        Term = input.Term,
                        Title = input.Title,
                        FirstName = input.FirstName,
                        LastName = input.LastName,
                        DateOfBirth = input.DateOfBirth,
                        Mobile = input.Mobile,
                        Email = input.Email,
                        ReferenceNo = new_reference_no,
                        DateOfApplication = clockService.LocalNow.Date,
                        FinanceRequestStatusId = initial_status?.FinanceRequestStatusId

                    }
                );

                financeRequest.Entity.FinanceRequestLog.Add(
                    new FinanceRequestLog
                    {
                        Title = "Creation",
                        Description = "Request",
                        Content = $"{financeRequest.Entity.FirstName} {financeRequest.Entity.LastName} has applied for a financing worth ${financeRequest.Entity.AmountRequired}. Reference No ${financeRequest.Entity.ReferenceNo}",
                        DateCreated = clockService.Now,
                    }
                );

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new RequestResponse
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                    ResponseMessage = $"{financeRequest.Entity.FinanceRequestId}"
                };

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in AddFishingViolationAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-AddFishingViolationAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }



        [UseDbContext(typeof(BankOn))]
        public async Task<Response> UpdateFinanceRequestAsync(
            FinanceRequest edit,
            [ScopedService] BankOn db,
            [Service] IConfiguration configuration,
            [Service] IClockService clockService,
            CancellationToken cancellationToken
        )
        {


            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var user_ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();


            try
            {


                var chosen_finance_request = db.FinanceRequest.FirstOrDefault(x => x.FinanceRequestId == edit.FinanceRequestId);
                var chosen_finance_product = db.FinanceProduct.FirstOrDefault(x => x.FinanceProductId == edit.FinanceProductId);

                if (chosen_finance_request is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.BadRequest),
                        ResponseLabel = "Client Error!",
                        ResponseMessage = "The finance request does not exist"
                    };

                }

                if (chosen_finance_product is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.BadRequest),
                        ResponseLabel = "Client Error!",
                        ResponseMessage = "The finance product of the request does not exist"
                    };

                }

                chosen_finance_request.DateOfBirth = edit.DateOfBirth;
                chosen_finance_request.FirstName = edit.FirstName;
                chosen_finance_request.LastName = edit.LastName;
                chosen_finance_request.Title = edit.Title;
                chosen_finance_request.AmountRequired = edit.AmountRequired;
                chosen_finance_request.Term = edit.Term;
                chosen_finance_request.FinanceProductId = edit.FinanceProductId;
                chosen_finance_request.FinanceRequestStatusId = edit.FinanceRequestStatusId;
                chosen_finance_request.FinanceRequestLog.Add(
                    new FinanceRequestLog
                    {
                        Title = "Update",
                        Description = "Request",
                        Content = $"{chosen_finance_request.FirstName} {chosen_finance_request.LastName} has updated their request ${chosen_finance_request.ReferenceNo}",
                        DateCreated = clockService.Now,
                    }
                );

                db.FinanceRequest.Update(chosen_finance_request);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new RequestResponse
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                    FRequestResponse = edit,
                };

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in UpdateFinanceRequestAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpdateFinanceRequestAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }

        [UseDbContext(typeof(BankOn))]
        public async Task<Response> ApplyFinanceRequestAsync(
            FinanceRequest edit,
            [ScopedService] BankOn db,
            [Service] IConfiguration configuration,
            [Service] IClockService clockService,
            CancellationToken cancellationToken
        )
        {


            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var user_ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();


            try
            {


                var chosen_finance_request = db.FinanceRequest.FirstOrDefault(x => x.FinanceRequestId == edit.FinanceRequestId);
                var chosen_finance_product = db.FinanceProduct.FirstOrDefault(x => x.FinanceProductId == edit.FinanceProductId);
                var submitted_status = db.FinanceRequestStatus.FirstOrDefault(x => x._case == 1);
                var denied_status = db.FinanceRequestStatus.FirstOrDefault(x => x._case == 3);

                var underEighteenFlag = false;
                var blackListedNumberFlag = false;
                var blackListedDomainFlag = false;


                if (chosen_finance_request is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.BadRequest),
                        ResponseLabel = "Client Error!",
                        ResponseMessage = "The finance request does not exist"
                    };
                }

                if (chosen_finance_product is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.BadRequest),
                        ResponseLabel = "Client Error!",
                        ResponseMessage = "The finance product of the request does not exist"
                    };
                }

                if (submitted_status is null || denied_status is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        ResponseLabel = "Status Error!",
                        ResponseMessage = "The required statuses and their cases have not been set"
                    };
                }

                LocalDate birthday = (LocalDate)chosen_finance_request.DateOfBirth;


                var period = Period.Between(birthday, clockService.LocalNow.Date, PeriodUnits.Years);

                if (period.Years < 18) underEighteenFlag = true;


                if (!underEighteenFlag && !blackListedDomainFlag && !blackListedNumberFlag)
                {
                    chosen_finance_request.TotalRepayment = edit.TotalRepayment;
                    chosen_finance_request.MonthlyRepayment = edit.MonthlyRepayment;
                    chosen_finance_request.FinanceRequestStatusId = submitted_status.FinanceRequestStatusId;

                    chosen_finance_request.FinanceRequestLog.Add(
                        new FinanceRequestLog
                        {
                            Title = "Submission",
                            Description = "Request",
                            Content = $"{chosen_finance_request.FirstName} {chosen_finance_request.LastName} has submitted their finance request ${chosen_finance_request.ReferenceNo}",
                            DateCreated = clockService.Now,
                        }
                    );

                }
                else
                {

                    chosen_finance_request.FinanceRequestStatusId = denied_status.FinanceRequestStatusId;

                    var deny_reason_1 = "";
                    var deny_reason_2 = "";
                    var deny_reason_3 = "";

                    if (underEighteenFlag) deny_reason_1 = "Applicant Under 18";
                    if (blackListedNumberFlag) deny_reason_2 = "Number Blacklisted";
                    if (blackListedDomainFlag) deny_reason_2 = "Email From Blacklisted Domain";

                    chosen_finance_request.FinanceRequestLog.Add(
                        new FinanceRequestLog
                        {
                            Title = "Denied",
                            Description = "Request",
                            Content = $"Finance Request ${chosen_finance_request.ReferenceNo} has been denied because of the following reasons: ${deny_reason_1} ${deny_reason_2} ${deny_reason_3}",
                            DateCreated = clockService.Now,
                        }
                    );

                }


                db.FinanceRequest.Update(chosen_finance_request);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new RequestResponse
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                    FRequestResponse = edit,
                };

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in UpdateFinanceRequestAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpdateFinanceRequestAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }


        [UseDbContext(typeof(BankOn))]
        public async Task<Response> UpsertFinanceProductsAsync(
            List<FinanceProduct> input,
            [ScopedService] BankOn db,
            CancellationToken cancellationToken,
            [Service] IClockService clockService
        )
        {
            var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var user_ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            try
            {

                // foreach (var residences in input)
                // {
                //     residences.modified_by_ip = user_ip;
                //     residences.modified_by = user.ToString();
                //     residences.date_modified = clockService.Now;
                // }


                await db.BulkInsertOrUpdateAsync(input, new BulkConfig
                {
                    SetOutputIdentity = true,
                    UpdateByProperties = new[] { "FinanceProductId" }.ToList(),
                    // PropertiesToExclude = new[] { "request" }.ToList()
                });

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new Response
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in UpsertFinanceProductsAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpsertFinanceProductsAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }


        [UseDbContext(typeof(BankOn))]
        public async Task<Response> UpsertFinanceRequestStatusesAsync(
            List<FinanceRequestStatus> input,
            [ScopedService] BankOn db,
            CancellationToken cancellationToken,
            [Service] IClockService clockService
        )
        {
            var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var user_ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            try
            {

                // foreach (var residences in input)
                // {
                //     residences.modified_by_ip = user_ip;
                //     residences.modified_by = user.ToString();
                //     residences.date_modified = clockService.Now;
                // }


                await db.BulkInsertOrUpdateAsync(input, new BulkConfig
                {
                    SetOutputIdentity = true,
                    UpdateByProperties = new[] { "FinanceRequestStatusId" }.ToList(),
                    // PropertiesToExclude = new[] { "request" }.ToList()
                });

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new Response
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in UpsertFishingViolatorResidencesAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpsertFishingViolatorResidencesAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }

    }
}