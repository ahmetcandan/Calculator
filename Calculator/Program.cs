using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string ifade = Console.ReadLine();
            while (ifade.ToUpper() != "EXIT")
            {
                Expression expression = new Expression(ifade);
                Console.WriteLine($"{expression.ToString()} = {expression.Value}");
                ifade = Console.ReadLine();
            }
        }
    }
}
