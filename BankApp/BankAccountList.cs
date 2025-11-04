
namespace BankApp
{
    public class BankAccountList
    {
        private readonly List<BankAccount> accounts = new List<BankAccount>();
        private int nextAccountNumber = 1;

        public IReadOnlyList<BankAccount> GetBankAccounts() => accounts.AsReadOnly();

        public BankAccount CreateAccount(decimal initialBalance = 0)
        {
            // generate a unique account number
            int newNumber = GenerateUniqueAccountNumber();
            var account = new BankAccount(newNumber, initialBalance);
            accounts.Add(account);
            return account;
        }

        private int GenerateUniqueAccountNumber()
        {
            // ensure the generated number is not taken
            while (accounts.Any(a => a.AccountNumber == nextAccountNumber))
            {
                nextAccountNumber++;
            }
            return nextAccountNumber++;
        }

        public void RemoveAccount(BankAccount account)
        {
            accounts.Remove(account);
        }

        public BankAccount GetAccountById(int accountId)
        {
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountId);
            return account ?? throw new ArgumentException("Account with given ID does not exist");
        }
    }
}
