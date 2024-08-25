using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Net;

namespace SubnetCalculator
{
    public class SubnetCalculator
    {
        public int BasePrefixLength { get; }
        private IPAddress BaseIp { get; }
        private uint BaseIpAsUint { get; }
        private List<Subnet> Subnets { get; }

        public SubnetCalculator(string cidrNotation, bool verboseMode = false)
        {
            (BaseIp, BasePrefixLength) = SubnetUtils.ParseCidrNotation(cidrNotation, verboseMode);
            BaseIpAsUint = SubnetUtils.IpToUint(BaseIp, verboseMode);
            Subnets = new List<Subnet>();

            if (verboseMode)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Initialized Subnet Calculator with:[/] [green]{BaseIp}[/] and prefix length [green]{BasePrefixLength}[/].");
            }
        }

        public void CalculateSubnets(List<int> subnetHosts, bool verboseMode = false)
        {
            uint currentIp = BaseIpAsUint;

            foreach (int hostCount in subnetHosts)
            {
                if (verboseMode)
                {
                    AnsiConsole.MarkupLine($"[bold yellow]Calculating subnet for [/] [green]{hostCount}[/] [bold yellow]hosts:[/]");
                }

                (uint subnetMask, uint subnetIncrement) = SubnetUtils.CalculateSubnetMaskAndIncrement(hostCount, verboseMode);

                Subnet subnet = new Subnet(currentIp, subnetMask, subnetIncrement, verboseMode);
                Subnets.Add(subnet);

                currentIp += subnetIncrement;

                if (verboseMode)
                {
                    AnsiConsole.MarkupLine($"[bold yellow]Finished calculating subnet.[/]");
                }
            }
        }

        public List<int> CalculateHostsPerSubnet(int subnetCount, bool verboseMode = false)
        {
            int hostsPerSubnet = (int)Math.Pow(2, 32 - BasePrefixLength) / subnetCount - 2;

            if (verboseMode)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Calculated hosts per subnet: [/] [green]{hostsPerSubnet}[/].");
            }

            return new List<int>(new int[subnetCount].Fill(hostsPerSubnet));
        }

        public void DisplaySubnets(bool verboseMode = false)
        {
            var table = new Table();
            table.AddColumn("Subnet #");
            table.AddColumn("Network ID");
            table.AddColumn("Subnet Mask");
            table.AddColumn("First Host");
            table.AddColumn("Last Host");
            table.AddColumn("Broadcast");

            foreach (Subnet subnet in Subnets)
            {
                table.AddRow(
                    (Subnets.IndexOf(subnet) + 1).ToString(),
                    subnet.NetworkId.ToString(),
                    subnet.SubnetMask.ToString(),
                    subnet.FirstHost.ToString(),
                    subnet.LastHost.ToString(),
                    subnet.Broadcast.ToString()
                );
            }

            AnsiConsole.Write(table);

            if (verboseMode)
            {
                AnsiConsole.MarkupLine("[bold yellow]Displayed all subnets in the table above.[/]");
            }
        }
    }
}
