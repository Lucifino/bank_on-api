using System;
using System.Collections.Generic;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class finance_product
    {
        public finance_product()
        {
            finance_request = new HashSet<finance_request>();
        }

        public Guid finance_product_id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public decimal? interest_rate { get; set; }
        public int? term_min { get; set; }
        public decimal? amount_min { get; set; }

        public virtual ICollection<finance_request> finance_request { get; set; }
    }
}
