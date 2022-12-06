namespace bank_on_api.Entites.Responses
{

    public class RequestResponse : Response
    {
        public bank_on_api.Models.Entities.BankOn.FinanceRequest? FRequestResponse { get; set; }
    }

    public class ProductResponse : Response
    {
        public bank_on_api.Models.Entities.BankOn.FinanceProduct? FProductResponse { get; set; }
    }

    public class StatusResponse : Response
    {
        public bank_on_api.Models.Entities.BankOn.FinanceRequestLog? FStatusResponse { get; set; }
    }


}

