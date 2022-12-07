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

                var savedRequest = db.FinanceRequest.FirstOrDefault(x => x.ReferenceNo == new_reference_no);

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

                    var addedRequest = db.FinanceRequest.FirstOrDefault(x => x.ReferenceNo == new_reference_no);

                    idParam = addedRequest?.FinanceRequestId.ToString();
                }




                return new RequestResponse
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                    ResponseMessage = $"{idParam}"
                };

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in AddFinanceRequestAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-AddFinanceRequestAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }



        [UseDbContext(typeof(BankOn))]
        public async Task<Response> UpdateFinanceRequestCustomerAsync(
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
                        ResponseCode = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        ResponseLabel = "Server Error!",
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

                chosen_finance_request.Mobile = edit.Mobile;
                chosen_finance_request.Email = edit.Email;
                chosen_finance_request.DateOfBirth = edit.DateOfBirth;
                chosen_finance_request.FirstName = edit.FirstName;
                chosen_finance_request.LastName = edit.LastName;
                chosen_finance_request.Title = edit.Title;
                chosen_finance_request.AmountRequired = edit.AmountRequired;
                chosen_finance_request.Term = edit.Term;
                chosen_finance_request.FinanceProductId = edit.FinanceProductId;
                chosen_finance_request.FinanceRequestLog.Add(
                    new FinanceRequestLog
                    {
                        Title = "Update Customer",
                        Description = "Request",
                        Content = $"{chosen_finance_request.FirstName} {chosen_finance_request.LastName} has updated the details of ticket {chosen_finance_request.ReferenceNo}",
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
                    ResponseMessage = $"{chosen_finance_request.ReferenceNo} quote has been generated",
                };

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in UpdateFinanceRequestCustomerAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpdateFinanceRequestCustomerAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }

        [UseDbContext(typeof(BankOn))]
        public async Task<Response> UpdateFinanceRequestAdminAsync(
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
                var approved_status = db.FinanceRequestStatus.FirstOrDefault(x => x._case == 2);
                var denied_status = db.FinanceRequestStatus.FirstOrDefault(x => x._case == 3);

                if (chosen_finance_request is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        ResponseLabel = "Server Error!",
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

                if (approved_status is null || denied_status is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.NotImplemented),
                        ResponseLabel = "Status Error!",
                        ResponseMessage = "The required statuses and their cases have not been set"
                    };
                }

                chosen_finance_request.ReferenceNo = edit.ReferenceNo;
                chosen_finance_request.Mobile = edit.Mobile;
                chosen_finance_request.Email = edit.Email;
                chosen_finance_request.DateOfBirth = edit.DateOfBirth;
                chosen_finance_request.FirstName = edit.FirstName;
                chosen_finance_request.LastName = edit.LastName;
                chosen_finance_request.Title = edit.Title;
                chosen_finance_request.AmountRequired = edit.AmountRequired;
                chosen_finance_request.Term = edit.Term;
                chosen_finance_request._UnderAgeFlag = edit._UnderAgeFlag;
                chosen_finance_request._BlackListDomainFlag = edit._BlackListDomainFlag;
                chosen_finance_request._BlackListMobileFlag = edit._BlackListMobileFlag;
                chosen_finance_request._Deleted = edit._Deleted;
                chosen_finance_request.FinanceRequestStatusId = edit.FinanceRequestStatusId;
                chosen_finance_request.FinanceProductId = edit.FinanceProductId;
                chosen_finance_request.MonthlyRepayment = edit.MonthlyRepayment;
                chosen_finance_request.TotalRepayment = edit.TotalRepayment;


                if (chosen_finance_request.FinanceRequestStatusId == approved_status.FinanceRequestStatusId)
                {
                    chosen_finance_request.FinanceRequestLog.Add(
                        new FinanceRequestLog
                        {
                            Title = "Update Admin",
                            Description = "Approved",
                            Content = $"Ticket {chosen_finance_request.ReferenceNo} has been approved by an Administrator",
                            DateCreated = clockService.Now,
                        }
                    );
                }
                else if (chosen_finance_request.FinanceRequestStatusId == denied_status.FinanceRequestStatusId)
                {
                    chosen_finance_request.FinanceRequestLog.Add(
                        new FinanceRequestLog
                        {
                            Title = "Update Admin",
                            Description = "Denied",
                            Content = $"Ticket {chosen_finance_request.ReferenceNo} has been denied by an Administrator",
                            DateCreated = clockService.Now,
                        }
                    );
                }
                else
                {
                    chosen_finance_request.FinanceRequestLog.Add(
                        new FinanceRequestLog
                        {
                            Title = "Update Admin",
                            Description = "Admin",
                            Content = $"Ticket {chosen_finance_request.ReferenceNo} has been updated by an Administrator",
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
                    ResponseMessage = $"You have updated ticket {chosen_finance_request.ReferenceNo}",
                };

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in UpdateFinanceRequestAdminAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpdateFinanceRequestAdminAsync-Exception")
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
                var mobileBlackList = db.BlackListGroup.FirstOrDefault(x => x._case == 1);
                var domainBlackList = db.BlackListGroup.FirstOrDefault(x => x._case == 2);

                var underEighteenFlag = false;
                var blackListedNumberFlag = false;
                var blackListedDomainFlag = false;


                if (chosen_finance_request is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        ResponseLabel = "Server Error!",
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
                        ResponseCode = Convert.ToInt32(HttpStatusCode.NotImplemented),
                        ResponseLabel = "Status Error!",
                        ResponseMessage = "The required statuses and their cases have not been set"
                    };
                }

                if (mobileBlackList is null || domainBlackList is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.NotImplemented),
                        ResponseLabel = "BlackList Error!",
                        ResponseMessage = "The blacklist and its cases have not been set"
                    };
                }

                chosen_finance_request.Mobile = edit.Mobile;
                chosen_finance_request.Email = edit.Email;
                chosen_finance_request.DateOfBirth = edit.DateOfBirth;
                chosen_finance_request.FirstName = edit.FirstName;
                chosen_finance_request.LastName = edit.LastName;
                chosen_finance_request.Title = edit.Title;
                chosen_finance_request.AmountRequired = edit.AmountRequired;
                chosen_finance_request.Term = edit.Term;
                chosen_finance_request._Deleted = edit._Deleted;
                chosen_finance_request.FinanceProductId = edit.FinanceProductId;

                List<string> mobileNumbers = new List<string>(mobileBlackList.Expression.Split(','));
                List<string> domains = new List<string>(domainBlackList.Expression.Split(','));

                LocalDate birthday = (LocalDate)chosen_finance_request.DateOfBirth;


                var period = Period.Between(birthday, clockService.LocalNow.Date, PeriodUnits.Years);
                var domain = chosen_finance_request.Email.Substring(chosen_finance_request.Email.IndexOf('@') + 1);

                if (period.Years < 18)
                {
                    underEighteenFlag = true;
                    chosen_finance_request._UnderAgeFlag = true;
                }

                mobileNumbers.ForEach(x =>
                {
                    if (x.Contains(chosen_finance_request.Mobile))
                    {
                        blackListedNumberFlag = true;
                        chosen_finance_request._BlackListMobileFlag = true;
                    }
                });

                domains.ForEach(x =>
                {
                    if (domain.Contains(x))
                    {
                        blackListedDomainFlag = true;
                        chosen_finance_request._BlackListDomainFlag = true;
                    }
                });

                //if (mobileNumbers.Contains(chosen_finance_request.Mobile))
                //{
                //    blackListedNumberFlag = true;
                //    chosen_finance_request._BlackListMobileFlag = true;
                //}


                //if (domains.Contains(domain))
                //{
                //    blackListedNumberFlag = true;
                //    chosen_finance_request._BlackListMobileFlag = true;
                //}

                var Message = "";



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
                            Content = $"{chosen_finance_request.FirstName} {chosen_finance_request.LastName} has submitted their finance request {chosen_finance_request.ReferenceNo}",
                            DateCreated = clockService.Now,
                        }
                    );

                    Message = $"Customer has submitted their ticket {chosen_finance_request.ReferenceNo}";

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
                            Content = $"Finance Request {chosen_finance_request.ReferenceNo} has been denied because of the following reasons: {deny_reason_1} {deny_reason_2} {deny_reason_3}",
                            DateCreated = clockService.Now,
                        }
                    );

                    Message = $"{chosen_finance_request.ReferenceNo} has alarming information in their profile. Financing has been denied";

                }


                db.FinanceRequest.Update(chosen_finance_request);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new RequestResponse
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                    ResponseMessage = Message
                };

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);

                var timestamp = clockService.InTicks;
                Log.Fatal(e, "Error in ApplyFinanceRequestAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-ApplyFinanceRequestAsync-Exception")
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
                Log.Fatal(e, "Error in UpsertFinanceRequestStatusesAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpsertFinanceRequestStatusesAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }

        [UseDbContext(typeof(BankOn))]
        public async Task<Response> UpsertBlackListGroupAsync(
            List<BlackListGroup> input,
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
                    UpdateByProperties = new[] { "BlackListGroupId" }.ToList(),
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
                Log.Fatal(e, "Error in UpsertBlackListGroupAsync Mutation by {User} {TimeStamp}", user_ip, timestamp);
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"There was an error while processing your request {timestamp}")
                    .SetCode("Mutation-UpsertBlackListGroupAsync-Exception")
                    .SetException(e)
                    .SetExtension("statusCode", (int)HttpStatusCode.InternalServerError)
                    .Build());
            }
        }

    }
}