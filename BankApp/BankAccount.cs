
namespace BankApp
{
    public class BankAccount
    {
        public decimal Balance { get; private set; }
        public int AccountNumber { get; private set; }

        internal BankAccount(int accountNumber, decimal initialBalance = 0)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public void Add(decimal amount)
        {
            if (amount == 0)
                throw new ArgumentException("Amount to add cannot be zero");
            if (amount < 0)
                throw new ArgumentException("Amount to add cannot be negative");

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount == 0)
                throw new ArgumentException("Amount to withdraw cannot be zero");
            if (amount < 0)
                throw new ArgumentException("Amount to withdraw cannot be negative");
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds");
            Balance -= amount;
        }

        public void TransferFundsTo(BankAccount targetAccount, decimal amount)
        {
            if (targetAccount == null)
                throw new ArgumentNullException(nameof(targetAccount));
            if (targetAccount == this) {
                throw new ArgumentException("Can't transfer funds to the same account");
            }
            Withdraw(amount);
            targetAccount.Add(amount);
        }
    }
}
