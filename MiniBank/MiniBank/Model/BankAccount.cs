using MiniBank.Interfaces;
using MiniBank.Model.enums;

namespace MiniBank.Model
{
    public abstract class BankAccount : ITransactable, IStatement
    {
        public int Id { get; } = new Random().Next(10, 99);
        public string Owner { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public List<Dictionary<DateTime,OperationType>> BankLog { get; private set; } = new();

        public BankAccount(string owner,decimal balace)
        {
            Owner = owner;
            Balance = balace;
            Console.WriteLine($"Created {Id} {this.GetType()} for {Owner} with {Balance} EUR");
            BankLog.Add(new Dictionary<DateTime, OperationType> { {DateTime.Now,OperationType.CREATE_ACCOUNT } });
        }

        public void Deposit(decimal amount)
        {
            this.Balance += amount;
            BankLog.Add(new Dictionary<DateTime, OperationType> { { DateTime.Now, OperationType.DEPOSIT } });
        }

        public void PrintStatement()
        {
            BankLog.Add(new Dictionary<DateTime, OperationType> { { DateTime.Now, OperationType.STATEMENT } });

            foreach (var logEntry in BankLog)
            {
                foreach (var entry in logEntry)
                {
                    Console.WriteLine($"Operation:{entry.Value} at {entry.Key}");
                }
            }
        }

        public virtual bool Withdraw(decimal amount, out string? error)
        {
            if (amount <= 0)
            {
                error = "Amount must be greater than zero.";
                return false;
            }

            if (amount > Balance)
            {
                error = "Insufficient funds.";
                return false;
            }

            Balance -= amount;
            error = null;
            BankLog.Add(new Dictionary<DateTime, OperationType> { { DateTime.Now, OperationType.WITHDRAW } });
            return true;
        }

        public override string ToString()
        {
            return $"Owner: {Owner} - {Id}\n Balance:{Balance}";
        }
    }
}
