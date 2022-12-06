

using NodaTime;

namespace bank_on_api.Models.Classes.BankOn
{
    public class FinanceRequestProxy
    {
        public decimal? AmountRequired { get; set; }
        public int? Term { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public LocalDate? DateOfBirth { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
    }
}