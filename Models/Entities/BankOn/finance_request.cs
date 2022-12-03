using System;
using System.Collections.Generic;
using NodaTime;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class finance_request
    {
        public finance_request()
        {
            finance_request_log = new HashSet<finance_request_log>();
        }

        public Guid finance_request_id { get; set; }
        public decimal? AmountRequired { get; set; }
        public int? Term { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public LocalDate? DateOfBirth { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ReferenceNo { get; set; }
        public Guid? finance_product_id { get; set; }
        public Guid? finance_request_status_id { get; set; }

        public virtual finance_product? finance_product { get; set; }
        public virtual finance_request_status? finance_request_status { get; set; }
        public virtual ICollection<finance_request_log> finance_request_log { get; set; }
    }
}
