using System;
using System.Collections.Generic;

namespace bank_on_api.Models.Entities.BankOn
{
    public partial class BlackListGroup
    {
        public Guid BlackListGroupId { get; set; }
        public string? Title { get; set; }
        public string? Expression { get; set; }
        public bool? _Deleted { get; set; }
        public int? _case { get; set; }
    }
}
