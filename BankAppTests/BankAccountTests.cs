using BankApp;

namespace BankAppTests
{
    //Tests for BankAccount and BankAccountList classes
    [TestFixture]
    public class BankAccountTests
    {
        private BankAccountList accounts;

        [SetUp]
        public void Setup()
        {
            //ARRANGE
            accounts = new BankAccountList();
        }

        [Test]
        public void BankAccountList_Starts_Empty()
        {
            //ASSERT
            Assert.That(accounts.GetBankAccounts(), Is.Empty);
        }

        [Test]
        public void Adding_New_Account()
        {

            //ACT
            BankAccount account = accounts.CreateAccount();
            //ASSERT
            Assert.That(account, Is.Not.Null);
            Assert.That(accounts.GetBankAccounts(), Does.Contain(account));
            Assert.That(account.AccountNumber, Is.GreaterThan(0));

        }

        [Test]
        // Checks if all accounts got unique numbers
        public void Creating_Multiple_Accounts_Generates_All_Unique_Numbers()
        {
            // ARRANGE + ACT
            var createdAccounts = Enumerable.Range(0, 100)
                                            .Select(_ => accounts.CreateAccount())
                                            .ToList();

            var accountNumbers = createdAccounts.Select(a => a.AccountNumber);
            // ASSERT
            Assert.That(accountNumbers.Distinct().Count(), Is.EqualTo(createdAccounts.Count));
        }

        [TestCase(500)]
        public void Creating_Account_With_Initial_Balance_Sets_Balance(decimal amount)
        {
            //ARRANGE + ACT
            var account = accounts.CreateAccount(amount);
            //ASSERT
            Assert.That(account.Balance, Is.EqualTo(amount));
        }

        [Test]
        public void Creating_Accounts_Generates_Unique_Numbers()
        {
            //ACT
            var account1 = accounts.CreateAccount();
            var account2 = accounts.CreateAccount();

            //ASSERT
            Assert.That(account1.AccountNumber, Is.Not.EqualTo(account2.AccountNumber));
        }

        [Test]
        public void Removing_Account()
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            var initialCount = accounts.GetBankAccounts().Count;

            //ACT
            accounts.RemoveAccount(account);

            //ASSERT
            Assert.That(accounts.GetBankAccounts().Count, Is.EqualTo(initialCount - 1));
            Assert.That(accounts.GetBankAccounts(), Does.Not.Contain(account));
        }

        [Test]
        public void Get_Account_By_ID()
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            var accountId = account.AccountNumber;

            //ACT + ASSERT
            Assert.That(accountId, Is.EqualTo(accounts.GetAccountById(accountId).AccountNumber));
        }

        [Test]
        public void Get_Account_By_Wrong_ID()
        {
            // ARRANGE
            var account = accounts.CreateAccount();
            var accountWrongId = account.AccountNumber + 1; // non-existing ID

            // ACT + ASSERT
            Assert.Throws<ArgumentException>(() => accounts.GetAccountById(accountWrongId));
            Assert.Throws<ArgumentException>(() => accounts.GetAccountById(-1));
            Assert.Throws<ArgumentException>(() => accounts.GetAccountById(int.MaxValue));
        }

        [TestCase(0)]
        public void Adding_Zero_Funds_Should_Throw_Exception(decimal amount)
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT + ASSERT
            Assert.Throws<ArgumentException>(() => account.Add(amount));
        }

        [TestCase(0)]
        public void Withdrawing_Zero_Funds_Should_Throw_Exception(decimal amount)
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT + ASSERT
            Assert.Throws<ArgumentException>(() => account.Withdraw(amount));
        }

        [Test]
        public void Adding_Decimal_Amounts_Keeps_Precision()
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT
            foreach (var _ in Enumerable.Range(0, 100))
                account.Add(0.01m);
            //ASSERT
            Assert.That(account.Balance, Is.EqualTo(1));
        }

        [TestCase(0.01)]
        [TestCase(100)]
        [TestCase(999.99)]
        public void Adding_Funds_With_Various_Amounts_Updates_Balance(decimal amount)
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT
            account.Add(amount);
            //ASSERT
            Assert.That(account.Balance, Is.EqualTo(amount));
        }

        [TestCase(0.01)]
        [TestCase(100)]
        [TestCase(999.99)]
        public void Adding_Negative_Funds_Throws_Exception(decimal amount)
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT + ASSERT
            Assert.Throws<ArgumentException>(() => account.Add(-amount));
        }

        [Test]
        public void Withdrawing_Funds_Updates_Balance()
        {
            //ARRANGE
            var account = accounts.CreateAccount(1000);
            //ACT
            account.Withdraw(500);
            //ASSERT
            Assert.That(account.Balance, Is.EqualTo(500));
        }

        [TestCase(0)]
        [TestCase(100)]
        [TestCase(999.99)]
        public void Withdrawing_Negative_Funds_Throws_Exception(decimal amount)
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT + ASSERT
            Assert.Throws<ArgumentException>(() => account.Withdraw(-amount));
        }

        [Test]
        public void Withdrawing_More_Than_Balance_Throws_Exception()
        {
            //ARRANGE
            var account = accounts.CreateAccount(1000);
            //ACT + ASSERT
            Assert.Throws<InvalidOperationException>(() => account.Withdraw(1500));
        }

        [Test]
        public void Batch_Add_Funds_To_All_Accounts()
        {
            //ARRANGE
            var accountsList = Enumerable.Range(0, 10)
                                         .Select(_ => accounts.CreateAccount())
                                         .ToList();
            //ACT
            accountsList.ForEach(a => a.Add(100));

            //ASSERT
            Assert.That(accountsList.All(a => a.Balance == 100));
        }

        [Test]
        public void Batch_Add_Negative_Funds_Throws_Exception()
        {
            //ARRANGE
            var accountsList = Enumerable.Range(0, 10)
                                         .Select(_ => accounts.CreateAccount())
                                         .ToList();

            //ACT + ASSERT
            foreach (var acc in accountsList)
            {
                Assert.Throws<ArgumentException>(() => acc.Add(-50));
            }
        }

        [Test]
        public void Adding_And_Withdrawing_In_Loop_Keeps_Correct_Balance()
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            var deposits = new[] { 100, 200, 300 };
            var withdrawals = new[] { 100, 200, 300 };

            //ACT
            foreach (var t in deposits)
            {
              account.Add(t);
            }
            foreach (var t in withdrawals)
            {
                account.Withdraw(t);
            }

            //ASSERT
            Assert.That(account.Balance, Is.EqualTo(deposits.Sum()- withdrawals.Sum())); // Should be 0
        }

        [Test]
        public void Transfering_Funds_Update_Both_Accounts()
        {
            //ARRANGE
            var account1 = accounts.CreateAccount(1000);
            var account2 = accounts.CreateAccount();
            //ACT
            account1.TransferFundsTo(account2, 500);
            //ASSERT
            Assert.That(account1.Balance, Is.EqualTo(500));
            Assert.That(account2.Balance, Is.EqualTo(500));
        }

        [Test]
        public void Multiple_Transfers_Keep_Correct_Balance()
        {
            //ARRANGE
            var a1 = accounts.CreateAccount(1000);
            var a2 = accounts.CreateAccount(500);
            var a3 = accounts.CreateAccount(200);

            //ACT
            a1.TransferFundsTo(a2, 300); // a1=700, a2=800
            a2.TransferFundsTo(a3, 400); // a2=400, a3=600

            //ASSERT
            Assert.That(a1.Balance, Is.EqualTo(700));
            Assert.That(a2.Balance, Is.EqualTo(400));
            Assert.That(a3.Balance, Is.EqualTo(600));
        }

        [Test]
        public void Transfer_To_Nonexisting_Account()
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT + ASSERT
            Assert.Throws<ArgumentNullException>(() => account.TransferFundsTo(null, 500));
        }

        [Test]
        public void Transfer_To_Same_Account()
        {
            //ARRANGE
            var account = accounts.CreateAccount();
            //ACT + ASSERT
            Assert.Throws<ArgumentException>(() => account.TransferFundsTo(account, 500));
        }
    }
}