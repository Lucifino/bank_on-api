using System.Net;
using bank_on_api.Entites.Responses;
using bank_on_api.Helpers;
using bank_on_api.Models.Entities.BankOn;
using EFCore.BulkExtensions;
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
            finance_request input,
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


                var initial_status = db.finance_request_status.FirstOrDefault(x => x._case == 0);
                var chosen_finance_product = db.finance_product.FirstOrDefault(x => x.finance_product_id == input.finance_product_id);

                if (initial_status is null)
                {
                    return new Response
                    {
                        ResponseCode = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        ResponseLabel = "Server Error!",
                        ResponseMessage = "Case 0 for finance request statuses has not been set"
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

                var new_long_code = Guid.NewGuid().ToString();
                var new_short_code = new_long_code.Substring(new_long_code.Length - 8);

                var product_id = input.finance_product_id.ToString();
                var product_code = product_id.Substring(product_id.Length - 8);

                var new_reference_no = $"{product_code}-{new_short_code}";

                input.ReferenceNo = new_reference_no;


                input.finance_request_log.Add(
                    new finance_request_log
                    {
                        title = "Creation",
                        description = "Request",
                        content = $"{input.FirstName} {input.LastName} has applied for a financing worth ${input.AmountRequired}. Reference No ${input.ReferenceNo}",
                        date_created = clockService.Now,
                    }
                );

                var list = new List<finance_request>
                    {input};

                await db.AddAsync(input);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new RequestResponse
                {
                    ResponseCode = Convert.ToInt32(HttpStatusCode.Accepted),
                    ResponseLabel = "Successful!",
                    FRequestResponse = input,
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
            finance_request edit,
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


                var chosen_finance_request = db.finance_request.FirstOrDefault(x => x.finance_request_id == edit.finance_request_id);
                var chosen_finance_product = db.finance_product.FirstOrDefault(x => x.finance_product_id == edit.finance_product_id);

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

                chosen_finance_request.AmountRequired = edit.AmountRequired;
                chosen_finance_request.Term = edit.Term;
                chosen_finance_request.finance_product_id = edit.finance_product_id;
                chosen_finance_request.finance_request_status_id = edit.finance_request_status_id;

                chosen_finance_request.finance_request_log.Add(
                    new finance_request_log
                    {
                        title = "Update",
                        description = "Request",
                        content = $"{chosen_finance_request.FirstName} {chosen_finance_request.LastName} has updated their request ${chosen_finance_request.ReferenceNo}",
                        date_created = clockService.Now,
                    }
                );

                db.finance_request.Update(chosen_finance_request);

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
            List<finance_product> input,
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
                    UpdateByProperties = new[] { "finance_product_id" }.ToList(),
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
            List<finance_request_status> input,
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
                    UpdateByProperties = new[] { "finance_request_status_id" }.ToList(),
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