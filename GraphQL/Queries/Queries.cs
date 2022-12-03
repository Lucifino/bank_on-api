
using bank_on_api.Models.Entities.BankOn;
using HotChocolate.AspNetCore.Authorization;

namespace bank_on_api.GraphQL.Queries
{
    public partial class Query
    {
        [UseDbContext(typeof(BankOn))]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_request> GetFinanceRequestsPaginated([ScopedService] BankOn context)
        {
            return context.finance_request;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_request> GetFinanceRequests([ScopedService] BankOn context)
        {
            return context.finance_request;
        }

        [UseDbContext(typeof(BankOn))]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_request_status> GetFinanceRequestStatusesPagnated([ScopedService] BankOn context)
        {
            return context.finance_request_status;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_request_status> GetFinanceRequestStatuses([ScopedService] BankOn context)
        {
            return context.finance_request_status;
        }

        [UseDbContext(typeof(BankOn))]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_request_log> GetFinanceRequestLogsPaginated([ScopedService] BankOn context)
        {
            return context.finance_request_log;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_request_log> GetFinanceRequestLogs([ScopedService] BankOn context)
        {
            return context.finance_request_log;
        }

        [UseDbContext(typeof(BankOn))]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_product> GetFinanceProductsPaginated([ScopedService] BankOn context)
        {
            return context.finance_product;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<finance_product> GetFinanceProducts([ScopedService] BankOn context)
        {
            return context.finance_product;
        }


    }
}