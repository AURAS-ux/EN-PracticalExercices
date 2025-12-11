using MiniBank.Interfaces;
using MiniBank.Model;

namespace MiniBank.Services
{
    public class AccountRegistry
    {
        public List<BankAccount> Accounts { get; set; } = new List<BankAccount>();
        public void  ListAccounts()
        {
            Console.WriteLine("=========== Listing accounts ===========");
            foreach (var account in Accounts)
            {
                Console.WriteLine($"Account nr:{account.Id} of owner: {account.Owner} with balance: {account.Balance}");
            }
            Console.WriteLine("=========== Finished listing accounts ===========");
        }

        public void CreateAccount()
        {
            Console.WriteLine("===== Creating account =====");
            while (true)
            {
                Console.Write("Specify account type(Checking, Savings, Loan):");
                string accountType = Console.ReadLine()!;
                if (accountType == null || accountType == string.Empty)
                {
                    Console.WriteLine("Account type is required. Please specify account type(Checking, Savings, Loan):");
                    continue;
                }

                Console.Write("Who will own this account?:");
                string owner = Console.ReadLine()!;

                if (owner == null || owner == string.Empty)
                {
                    Console.WriteLine("Owner is required. Please specify who will own this account:");
                    continue;
                }

                Console.Write("What is the initial deposit amount?:");
                if (!decimal.TryParse(Console.ReadLine(), out decimal initialDeposit))
                {
                    Console.WriteLine("Initial deposit amount is required. Please specify the initial deposit amount:");
                    continue;
                }

                switch(accountType.ToLower())
                {
                    case "checking":
                        Accounts.Add(new CheckingAccount(owner,initialDeposit));
                        break;
                    case "savings":
                        Accounts.Add(new SavingsAccount(owner,initialDeposit));
                        break;
                    case "loan":
                        Accounts.Add(new LoanAccount(owner,initialDeposit));
                        break;
                    default:
                        Console.WriteLine("Invalid account type. Please specify account type(Checking, Savings, Loan):");
                        continue;
                }

                Console.WriteLine("===== Account creation finished =====");
                break;
            }
        }

        public void InitWithdraw()
        {
            Console.WriteLine("====== Withdraw Initiated ======");
            do
            {
                Console.WriteLine("Please specify your account id:");
                int inputId = int.TryParse(Console.ReadLine(), out int id) ? id : -1;
                if(inputId == -1)
                {
                    Console.WriteLine("Invalid account id. Please try again.");
                    continue;
                }
                Console.WriteLine($"Initiated search for account with ID: {inputId}");
                BankAccount? userAccount;
                foreach (var account in Accounts)
                {
                    if(account.Id == inputId)
                    {
                        userAccount = account;
                        Console.WriteLine($"Found account {userAccount.Id} of owner {userAccount.Owner} with balance {userAccount.Balance}");
                        do
                        {
                            Console.WriteLine("Please specify the amount to withdraw:");
                            if(!decimal.TryParse(Console.ReadLine(), out decimal amount))
                            {
                                Console.WriteLine("Invalid amount. Please try again.");
                                continue;
                            }
                            if(userAccount.Withdraw(amount, out string? error))
                            {
                                Console.WriteLine($"Withdrawal of {amount} successful. New balance is {userAccount.Balance}");
                            }
                            else
                            {
                                Console.WriteLine($"Withdrawal failed: {error}");
                            }
                            Console.WriteLine("====== Withdraw Finished ======");
                            return;
                        }
                        while (true);
                    }
                }
            }
            while (true);
        }

        public void InitDeposit()
        {
            Console.WriteLine("==== Deposit Initiated ====");
            do
            {
                Console.WriteLine("Please specify your account id:");
                int inputId = int.TryParse(Console.ReadLine(), out int id) ? id : -1;
                if (inputId == -1)
                {
                    Console.WriteLine("Invalid account id. Please try again.");
                    continue;
                }
                Console.WriteLine($"Initiated search for account with ID: {inputId}");
                BankAccount? userAccount;
                foreach (var account in Accounts)
                {
                    if (account.Id == inputId)
                    {
                        userAccount = account;
                        Console.WriteLine($"Found account {userAccount.Id} of owner {userAccount.Owner} with balance {userAccount.Balance}");
                        do
                        {
                            Console.WriteLine("Please specify the amount to deposit:");
                            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                            {
                                Console.WriteLine("Invalid amount. Please try again.");
                                continue;
                            }
                            userAccount.Deposit(amount);
                            Console.WriteLine("====== Deposit Finished ======");
                            return;
                        }
                        while (true);
                    }
                }
            }
            while (true);
        }

        public void RequestStatement()
        {
            Console.WriteLine("==== Statement Request Initiated ====");
            do
            {
                Console.WriteLine("Please specify your account id:");
                int inputId = int.TryParse(Console.ReadLine(), out int id) ? id : -1;
                if (inputId == -1)
                {
                    Console.WriteLine("Invalid account id. Please try again.");
                    continue;
                }
                Console.WriteLine($"Initiated search for account with ID: {inputId}");
                BankAccount? userAccount;
                foreach (var account in Accounts)
                {
                    if (account.Id == inputId)
                    {
                        userAccount = account;
                        Console.WriteLine($"Found account {userAccount.Id} of owner {userAccount.Owner} with balance {userAccount.Balance}");
                        userAccount.PrintStatement();
                        Console.WriteLine("====== Statement Request Finished ======");
                        return;
                    }
                }
            }
            while (true);
        }

        public void RunMonthEnd()
        {
            Console.WriteLine("==== Running Month End Process ====");

            foreach (var account in Accounts.OfType<IInterestBearing>())
            {
                account.ApplyMonthlyInterest();
            }

            Console.WriteLine("==== Month End Process Finished ====");
        }
    }
}
