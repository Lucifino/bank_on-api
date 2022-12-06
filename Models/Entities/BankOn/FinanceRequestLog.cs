using System;
using System.Collections.Generic;
using NodaTime;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class FinanceRequestLog
    {
        public Guid FinanceRequestLogId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? FinanceRequestId { get; set; }
        public string? Content { get; set; }
        public Instant? DateCreated { get; set; }
        public bool? _Deleted { get; set; }

        public virtual FinanceRequest? FinanceRequest { get; set; }
    }
}
