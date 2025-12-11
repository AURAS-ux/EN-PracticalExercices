using MiniBank.Interfaces;
using MiniBank.Model.enums;

namespace MiniBank.Model
{
    public class CheckingAccount : BankAccount, IOverdraftPolicy
    {
        public decimal OverdraftLimit { get; } = Constants.overdraftLimit;

        public CheckingAccount(string owner, decimal balance) : base(owner, balance)
        {
        }

        public override bool Withdraw(decimal amount, out string? error)
        {
            if(amount <= 0)
            {
                error = "Amount must be greater than zero.";
                return false;
            }

            if(Balance - amount < OverdraftLimit)
            {
                error = "Withdrawal would exceed overdraft limit.";
                return false;
            }

            Balance -= amount;
            error = null;
            BankLog.Add(new Dictionary<DateTime, OperationType> { { DateTime.Now, OperationType.WITHDRAW } });
            return true;
        }
    }
}
