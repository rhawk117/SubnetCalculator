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
            bool verboseMode = AppUI.GetVerboseMode();
            SubnetCalculator calculator = AppUI.InitializeCalculator(verboseMode);

            if (calculator != null)
            {
                calculator.DisplaySubnets(verboseMode);
            }
        }
    }
}
