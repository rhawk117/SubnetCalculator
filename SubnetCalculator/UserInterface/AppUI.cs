using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SubnetCalculator
{
    public static class AppUI
    {
        private static readonly Regex CidrRegex = new Regex(@"^(\d{1,3}\.){3}\d{1,3}/\d{1,2}$");

        private const string HEADER = @"

=========================================================================================================

           ________  ___  ___  ________  ________   _______  _________                                       
          |\   ____\|\  \|\  \|\   __  \|\   ___  \|\  ___ \|\___   ___\                                     
          \ \  \___|\ \  \\\  \ \  \|\ /\ \  \\ \  \ \   __/\|___ \  \_|                                     
           \ \_____  \ \  \\\  \ \   __  \ \  \\ \  \ \  \_|/__  \ \  \                                      
            \|____|\  \ \  \\\  \ \  \|\  \ \  \\ \  \ \  \_|\ \  \ \  \                                     
              ____\_\  \ \_______\ \_______\ \__\\ \__\ \_______\  \ \__\                                    
             |\_________\|_______|\|_______|\|__| \|__|\|_______|   \|__|                                    
             \|_________|                                                                                    
                                                                                                             
                                                                                                             
 ________  ________  ___       ________  ___  ___  ___       ________  _________  ________  ________         
|\   ____\|\   __  \|\  \     |\   ____\|\  \|\  \|\  \     |\   __  \|\___   ___\\   __  \|\   __  \        
\ \  \___|\ \  \|\  \ \  \    \ \  \___|\ \  \\\  \ \  \    \ \  \|\  \|___ \  \_\ \  \|\  \ \  \|\  \       
 \ \  \    \ \   __  \ \  \    \ \  \    \ \  \\\  \ \  \    \ \   __  \   \ \  \ \ \  \\\  \ \   _  _\      
  \ \  \____\ \  \ \  \ \  \____\ \  \____\ \  \\\  \ \  \____\ \  \ \  \   \ \  \ \ \  \\\  \ \  \\  \|     
   \ \_______\ \__\ \__\ \_______\ \_______\ \_______\ \_______\ \__\ \__\   \ \__\ \ \_______\ \__\\ _\     
    \|_______|\|__|\|__|\|_______|\|_______|\|_______|\|_______|\|__|\|__|    \|__|  \|_______|\|__|\|__|    
                                                                                                             
                                Made By: rhawk117
                          [ PRESS ENTER TO CONTINUE... }
=========================================================================================================
";


        public static void DisplayHeader()
        {
            Console.WriteLine(HEADER);
            Console.ReadKey();
            Console.Clear();
        }


        public static string GetSubnettingChoice()
        {
            Prompts.Question("Select a Subnetting Type:");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold green]Select an Option Below[/]")
                    .AddChoices(new[] { "Fixed Length Subnetting", "Variable Length Subnetting" }));

            return choice == "Fixed Length Subnetting" ? "1" : "2";
        }

        public static bool GetVerboseMode()
        {
            string prompt = "Would you like to enable verbose mode? (This will walk you through the steps of calculating the subnets)";
            return AnsiConsole.Confirm($"[bold green]{prompt}[/]");
        }


        public static SubnetCalculator InitializeCalculator(bool verboseMode)
        {
            string divider = new string('=', 80);
            Console.WriteLine(divider);
            Prompts.InfoMessage("Welcome to the Subnet Calculator! (developed by: rhawk117)");
            Console.WriteLine(divider);
            string choice = GetSubnettingChoice();
            string cidrNotation = GetValidCidrNotation();

            Console.WriteLine(divider);
            SubnetCalculator calculator = new SubnetCalculator(cidrNotation, verboseMode);

            if (choice == "1") handleFixedLength(calculator, verboseMode);

            else if (choice == "2") handleVariableLength(calculator, verboseMode);

            return calculator;
        }

        private static void handleFixedLength(SubnetCalculator calculator, bool verboseMode)
        {
            int subnetCount = GetValidSubnetCount(calculator.BasePrefixLength);
            List<int> subnetHosts = calculator.CalculateHostsPerSubnet(subnetCount, verboseMode);
            calculator.CalculateSubnets(subnetHosts, verboseMode);
        }

        private static void handleVariableLength(SubnetCalculator calculator, bool verboseMode)
        {
            List<int> subnetHosts = GetVariableLengthHosts(calculator.BasePrefixLength);
            calculator.CalculateSubnets(subnetHosts, verboseMode);
        }

        public static Table GenerateOuput()
        {
            Table table = new Table();
            table.AddColumn("Subnet #");
            table.AddColumn("Network ID");
            table.AddColumn("Subnet Mask");
            table.AddColumn("First Host");
            table.AddColumn("Last Host");
            table.AddColumn("Broadcast");
            return table;
        }

        public static string GetValidCidrNotation()
        {
            string prompt = "Enter a valid IP address in CIDR notation you'd like to subnet (e.g., 192.168.1.0/24):";
            while (true)
            {

                string cidrNotation = AnsiConsole.Ask<string>($"[bold green]{prompt}[/]");

                if (CidrRegex.IsMatch(cidrNotation) == false)
                {
                    Prompts.Error("The IP Address provided is not in CIDR notation. Please try again.");
                    continue;
                }
                string[] parts = cidrNotation.Split('/');
                string ipAddress = parts[0];
                int prefixLength = int.Parse(parts[1]);

                if (IsValidIpAddress(ipAddress) && prefixLength >= 0 && prefixLength <= 32)
                {
                    return cidrNotation;
                }
            }
        }

        public static int GetValidSubnetCount(int prefixLength)
        {
            int maxSubnets = (int)Math.Pow(2, 32 - prefixLength);
            string prompt = $"Enter the number of subnets needed (Max: {maxSubnets}):";
            while (true)
            {
                int subnetCount = AnsiConsole.Ask<int>($"[bold blue]{prompt}[/]");
                if (subnetCount > 0 && subnetCount <= maxSubnets)
                {
                    return subnetCount;
                }
                Prompts.Error($"Invalid number of subnets.Please enter a value between 1 and {maxSubnets}.");
            }
        }

        public static List<int> GetVariableLengthHosts(int prefixLength)
        {
            int maxHosts = (int)Math.Pow(2, 32 - prefixLength) - 2;
            int subnetCount = GetValidSubnetCount(prefixLength);
            List<int> subnetHosts = new List<int>();

            for (int i = 0; i < subnetCount; i++)
            {
                GetSubnetHostCount(subnetHosts, maxHosts, i);
            }

            return subnetHosts;
        }

        public static void GetSubnetHostCount(List<int> subnetHosts, int maxHosts, int iter)
        {
            string prompt = $"Enter the number of hosts for subnet {iter + 1} (Max: {maxHosts}):";
            while (true)
            {
                int hosts = AnsiConsole.Ask<int>($"[bold yellow]{prompt}[/]");
                if (hosts > 0 && hosts <= maxHosts)
                {
                    subnetHosts.Add(hosts);
                    break;
                }
                Prompts.Error($"The number of hosts you provided exceeded the maximum number of hosts allowed for your subnet. \n Please enter a value between 1 and {maxHosts}.");
            }
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
