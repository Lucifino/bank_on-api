using System;
using System.Collections.Generic;
using NodaTime;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class finance_request_log
    {
        public Guid finance_request_log_id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public int? _case { get; set; }
        public Guid? finance_request_id { get; set; }
        public string? content { get; set; }
        public Instant? date_created { get; set; }

        public virtual finance_request? finance_request { get; set; }
    }
}
