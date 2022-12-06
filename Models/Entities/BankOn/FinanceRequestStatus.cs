using System;
using System.Collections.Generic;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class FinanceRequestStatus
    {
        public FinanceRequestStatus()
        {
            FinanceRequest = new HashSet<FinanceRequest>();
        }

        public Guid FinanceRequestStatusId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? _case { get; set; }
        public bool? _Deleted { get; set; }

        public virtual ICollection<FinanceRequest> FinanceRequest { get; set; }
    }
}
