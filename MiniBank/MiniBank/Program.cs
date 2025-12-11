using MiniBank.Services;

Console.WriteLine("======= MINIBANK =======");
bool running = true;
AccountRegistry registry = new AccountRegistry();

do
{
    Console.WriteLine("1. List accounts");
    Console.WriteLine("2. Create account");
    Console.WriteLine("3. Deposit");
    Console.WriteLine("4. Withdraw");
    Console.WriteLine("5. View statement");
    Console.WriteLine("6. Run month-end");
    Console.WriteLine("7. Exit");

    Console.Write("Select:");
    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Please enter a number from the above menu:");
        continue;
    }

    switch (choice)
    {
        case 1:
            registry.ListAccounts();
            break;
        case 2:
            registry.CreateAccount();
            break;
        case 3:
            registry.InitDeposit();
            break;
        case 4:
            registry.InitWithdraw();
            break;
        case 5:
            registry.RequestStatement();
            break;
        case 6:
            registry.RunMonthEnd();
            break;
        case 7:
            running = false;
            break;
    }
} while (running);