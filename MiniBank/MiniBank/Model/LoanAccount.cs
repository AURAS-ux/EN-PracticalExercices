using MiniBank.Interfaces;
using MiniBank.Model.enums;

namespace MiniBank.Model
{
    public class LoanAccount : BankAccount, IInterestBearing
    {
        public LoanAccount(string owner, decimal balance) : base(owner, balance)
        {
           
        }
        public void ApplyMonthlyInterest()
        {
            if(Balance < 0)
            {
                Balance += Balance * Constants.loanInterestRate;
                BankLog.Add(new Dictionary<DateTime, OperationType> { { DateTime.Now, OperationType.INTEREST_LOAN } });
            }
        }

        public override bool Withdraw(decimal amount, out string? error)
        {
            if(amount <= 0)
            {
                error = "Amount must be greater than zero.";
                return false;
            }

            Balance -= amount;
            error = null;
            BankLog.Add(new Dictionary<DateTime, OperationType> { { DateTime.Now, OperationType.WITHDRAW } });
            return true;
        }
    }
}
