namespace CreepyDonut.Models
{
    public class CheckoutRequest
    {
        public string payment_type { get; set; }
        public TransactionDetails transaction_details { get; set; }
        public CreditCard credit_card { get; set; }
        public CustomerDetails customer_details { get; set; }
    }

    public class TransactionDetails
    {
        public string order_id { get; set; }
        public int gross_amount { get; set; }
    }

    public class CreditCard
    {
        public bool authentication { get; set; } // ganti dari "secure"
        public string token_id { get; set; }     // token dari Snap atau dummy
    }


    public class CustomerDetails
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
}
