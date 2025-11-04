using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public class PaymentValidator
    {
        public PaymentResult ValidatePaymentInput(string userId, string vendorId, decimal amount)
        {
            var result = new PaymentResult { Success = true, ErrorMessage = string.Empty };
            if (string.IsNullOrWhiteSpace(userId))
                return PaymentResult.Fail("User ID cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(vendorId))
                return PaymentResult.Fail("Vendor ID cannot be null or empty.");

            if (amount <= 0)
                return PaymentResult.Fail("Amount must be positive.");
            return result;
        }
    }
}
