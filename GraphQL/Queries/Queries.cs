
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
        public IQueryable<FinanceRequest> GetFinanceRequestsPaginated([ScopedService] BankOn context)
        {
            return context.FinanceRequest;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<FinanceRequest> GetFinanceRequests([ScopedService] BankOn context)
        {
            return context.FinanceRequest;
        }

        [UseDbContext(typeof(BankOn))]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<FinanceRequestStatus> GetFinanceRequestStatusesPagnated([ScopedService] BankOn context)
        {
            return context.FinanceRequestStatus;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<FinanceRequestStatus> GetFinanceRequestStatuses([ScopedService] BankOn context)
        {
            return context.FinanceRequestStatus;
        }

        [UseDbContext(typeof(BankOn))]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<FinanceRequestLog> GetFinanceRequestLogsPaginated([ScopedService] BankOn context)
        {
            return context.FinanceRequestLog;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<FinanceRequestLog> GetFinanceRequestLogs([ScopedService] BankOn context)
        {
            return context.FinanceRequestLog;
        }

        [UseDbContext(typeof(BankOn))]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<FinanceProduct> GetFinanceProductsPaginated([ScopedService] BankOn context)
        {
            return context.FinanceProduct;
        }

        [UseDbContext(typeof(BankOn))]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<FinanceProduct> GetFinanceProducts([ScopedService] BankOn context)
        {
            return context.FinanceProduct;
        }


    }
}