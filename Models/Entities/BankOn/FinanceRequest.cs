using System;
using System.Collections.Generic;
using NodaTime;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class FinanceRequest
    {
        public FinanceRequest()
        {
            FinanceRequestLog = new HashSet<FinanceRequestLog>();
        }

        public Guid FinanceRequestId { get; set; }
        public decimal? AmountRequired { get; set; }
        public int? Term { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public LocalDate? DateOfBirth { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ReferenceNo { get; set; }
        public Guid? FinanceProductId { get; set; }
        public Guid? FinanceRequestStatusId { get; set; }
        public LocalDate? DateOfApplication { get; set; }
        public bool? _Deleted { get; set; }
        public decimal? MonthlyRepayment { get; set; }
        public decimal? TotalRepayment { get; set; }

        public virtual FinanceProduct? FinanceProduct { get; set; }
        public virtual FinanceRequestStatus? FinanceRequestStatus { get; set; }
        public virtual ICollection<FinanceRequestLog> FinanceRequestLog { get; set; }
    }
}
