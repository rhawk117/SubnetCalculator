using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubnetCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppUI.DisplayHeader();
            bool showVerboseOutput = AppUI.GetVerboseMode();
            SubnetCalculator calculator = AppUI.InitializeCalculator(showVerboseOutput);
            if (calculator == null)
            {
                Console.WriteLine($"(!) Something went horribly wrong please try again (!)");
            }
            calculator.DisplaySubnets(showVerboseOutput);
        }
    }
}
