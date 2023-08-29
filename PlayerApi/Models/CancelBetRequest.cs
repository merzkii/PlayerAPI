namespace PlayerApi.Models
{
    public class CancelBetRequest:BetRequest
    {
        public string BetTransactionId { get; set; }
    }
}
