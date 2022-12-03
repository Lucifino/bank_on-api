using System;
using System.Collections.Generic;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class finance_request_status
    {
        public finance_request_status()
        {
            finance_request = new HashSet<finance_request>();
        }

        public Guid finance_request_status_id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public int? _case { get; set; }

        public virtual ICollection<finance_request> finance_request { get; set; }
    }
}
