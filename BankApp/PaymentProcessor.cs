
namespace BankApp
{
    public interface IVendorDatabase
    {
        PaymentResult EnsureVendorExists(string vendorId);
    }

    public interface ISslService
    {
        PaymentResult PerformHandshake(string vendorId);
    }

    public interface IUserConsentService
    {
        PaymentResult VerifyUserConsent(string userId, decimal amount);
    }


    public class PaymentProcessor
    {
        private readonly IVendorDatabase _vendorDatabase;
        private readonly ISslService _sslService;
        private readonly IUserConsentService _consentService;
        private readonly PaymentValidator _validator;

        public PaymentProcessor(
            IVendorDatabase vendorDatabase,
            ISslService sslService,
            IUserConsentService consentService,
            PaymentValidator validator)
        {
            _vendorDatabase = vendorDatabase;
            _sslService = sslService;
            _consentService = consentService;
            _validator = validator;
        }

        public PaymentResult ProcessPayment(string userId, string vendorId, decimal amount)
        {
            // Validate input first
            var validation = _validator.ValidatePaymentInput(userId, vendorId, amount);
            if (!validation.Success) return validation;

            // Check vendor
            var vendorResult = _vendorDatabase.EnsureVendorExists(vendorId);
            if (!vendorResult.Success) return vendorResult;

            // SSL handshake
            var sslResult = _sslService.PerformHandshake(vendorId);
            if (!sslResult.Success) return sslResult;

            // User consent
            var consentResult = _consentService.VerifyUserConsent(userId, amount);
            if (!consentResult.Success) return consentResult;

            // Payment succeeded
            Console.WriteLine($"✅ Payment of {amount:C} from {userId} to {vendorId} succeeded.");
            return PaymentResult.Ok();
        }
    }
}
