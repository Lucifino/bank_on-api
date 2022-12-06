using System;
using System.Collections.Generic;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class FinanceProduct
    {
        public FinanceProduct()
        {
            FinanceRequest = new HashSet<FinanceRequest>();
        }

        public Guid FinanceProductId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? InterestRate { get; set; }
        public int? TermMin { get; set; }
        public decimal? AmountMin { get; set; }
        public bool? _Deleted { get; set; }

        public virtual ICollection<FinanceRequest> FinanceRequest { get; set; }
    }
}
