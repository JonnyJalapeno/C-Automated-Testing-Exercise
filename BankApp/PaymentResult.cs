
namespace BankApp
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static PaymentResult Ok() => new PaymentResult { Success = true };
        public static PaymentResult Fail(string message) => new PaymentResult { Success = false, ErrorMessage = message };
    }
}
