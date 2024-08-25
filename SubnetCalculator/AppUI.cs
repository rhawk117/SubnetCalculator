using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubnetCalculator
{
    public static class AppUI
    {
        private static readonly Regex CidrRegex = new Regex(@"^(\d{1,3}\.){3}\d{1,3}/\d{1,2}$");

        public static string GetSubnettingChoice()
        {
            AnsiConsole.MarkupLine("[bold yellow]Choose Subnetting Type:[/]");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Please select an option:[/]")
                    .AddChoices(new[] { "Fixed Length Subnetting", "Variable Length Subnetting" }));
            return choice == "Fixed Length Subnetting" ? "1" : "2";
        }

        public static bool GetVerboseMode()
        {
            return AnsiConsole.Confirm("[bold yellow]Would you like to enable verbose mode? (This will walk you through the steps of calculating the subnets)[/]");
        }

        public static SubnetCalculator InitializeCalculator(bool verboseMode)
        {
            AnsiConsole.MarkupLine("[bold yellow]Welcome to the Subnet Calculator![/]");
            string choice = GetSubnettingChoice();
            string cidrNotation = GetValidCidrNotation();

            SubnetCalculator calculator = new SubnetCalculator(cidrNotation, verboseMode);

            if (choice == "1")
            {
                int subnetCount = GetValidSubnetCount(calculator.BasePrefixLength);
                List<int> subnetHosts = calculator.CalculateHostsPerSubnet(subnetCount, verboseMode);
                calculator.CalculateSubnets(subnetHosts, verboseMode);
            }
            else if (choice == "2")
            {
                List<int> subnetHosts = GetVariableLengthHosts(calculator.BasePrefixLength);
                calculator.CalculateSubnets(subnetHosts, verboseMode);
            }

            return calculator;
        }

        public static string GetValidCidrNotation()
        {
            while (true)
            {
                string cidrNotation = AnsiConsole.Ask<string>("[bold yellow]Enter IP address with CIDR notation (e.g., 192.168.1.0/24):[/]");

                if (CidrRegex.IsMatch(cidrNotation))
                {
                    string[] parts = cidrNotation.Split('/');
                    string ipAddress = parts[0];
                    int prefixLength = int.Parse(parts[1]);

                    if (IsValidIpAddress(ipAddress) && prefixLength >= 0 && prefixLength <= 32)
                    {
                        return cidrNotation;
                    }
                }

                AnsiConsole.MarkupLine("[bold red]Invalid CIDR notation. Please try again.[/]");
            }
        }

        public static int GetValidSubnetCount(int prefixLength)
        {
            int maxSubnets = (int)Math.Pow(2, 32 - prefixLength);

            while (true)
            {
                int subnetCount = AnsiConsole.Ask<int>($"[bold yellow]Enter the number of subnets needed (Max: {maxSubnets}):[/]");
                if (subnetCount > 0 && subnetCount <= maxSubnets)
                {
                    return subnetCount;
                }

                AnsiConsole.MarkupLine($"[bold red]Invalid number of subnets. Please enter a value between 1 and {maxSubnets}.[/]");
            }
        }

        public static List<int> GetVariableLengthHosts(int prefixLength)
        {
            int maxHosts = (int)Math.Pow(2, 32 - prefixLength) - 2;
            int subnetCount = GetValidSubnetCount(prefixLength);
            List<int> subnetHosts = new List<int>();

            for (int i = 0; i < subnetCount; i++)
            {
                while (true)
                {
                    int hosts = AnsiConsole.Ask<int>($"[bold yellow]Enter the number of hosts for subnet {i + 1} (Max: {maxHosts}):[/]");
                    if (hosts > 0 && hosts <= maxHosts)
                    {
                        subnetHosts.Add(hosts);
                        break;
                    }

                    AnsiConsole.MarkupLine($"[bold red]Invalid number of hosts. Please enter a value between 1 and {maxHosts}.[/]");
                }
            }

            return subnetHosts;
        }

        private static bool IsValidIpAddress(string ipAddress)
        {
            string[] octets = ipAddress.Split('.');
            foreach (string octet in octets)
            {
                if (!int.TryParse(octet, out int value) || value < 0 || value > 255)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
