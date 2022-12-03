namespace bank_on_api.Entites.Responses
{

    public class RequestResponse : Response
    {
        public bank_on_api.Models.Entities.BankOn.finance_request? FRequestResponse { get; set; }
    }

    public class ProductResponse : Response
    {
        public bank_on_api.Models.Entities.BankOn.finance_product? FProductResponse { get; set; }
    }

    public class StatusResponse : Response
    {
        public bank_on_api.Models.Entities.BankOn.finance_request_status? FStatusResponse { get; set; }
    }


}

