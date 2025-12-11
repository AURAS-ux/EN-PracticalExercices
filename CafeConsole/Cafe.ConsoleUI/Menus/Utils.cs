using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.ConsoleUI.Menus
{
    public static class Utils
    {
        public static string RequestInput()
        {
            string? input = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                input = Console.ReadLine();
            }
            return input!;
        }
    }
}
