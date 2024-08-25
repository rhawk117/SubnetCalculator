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
            (BaseIp, BasePrefixLength) = SubnetUtils.ParseCIDR(cidrNotation, verboseMode);
            BaseIpAsUint = SubnetUtils.IpToUint(BaseIp, verboseMode);
            Subnets = new List<Subnet>();

            Prompts.DisplayIfVerbose(verboseMode, () =>
                Prompts.VerboseMessage(
                    $"\n[bold blue] (*) Initialized Subnet Calculator with: [/] [green italic]{BaseIp}[/] & prefix length [green italic]{BasePrefixLength}[/].")
            );

        }

        public void CalculateSubnets(List<int> subnetHosts, bool verboseMode = false)
        {
            uint currentIp = BaseIpAsUint;

            foreach (int hostCount in subnetHosts)
            {
                Prompts.DisplayIfVerbose(verboseMode, () =>
                    Prompts.VerboseMessage(
                        $"(*) [bold blue] Calculating subnet for [/] [green italic]{hostCount}[/] [bold yellow] hosts[/]"
                    )
                );

                (uint subnetMask, uint subnetIncrement) = SubnetUtils
                                                         .CalcSubnetMask(hostCount,
                                                         verboseMode);
                Subnets.Add(new Subnet(currentIp, subnetMask, subnetIncrement,
                verboseMode));

                currentIp += subnetIncrement;

                Prompts.DisplayIfVerbose(verboseMode, () =>
                    Prompts.VerboseMessage("[green italic](*) Finished calculating subnet... (*)[/]")
                );
            }
        }

        public List<int> CalculateHostsPerSubnet(int subnetCount, bool verboseMode = false)
        {
            int hostsPerSubnet = (int)Math.Pow(2, 32 - BasePrefixLength) / subnetCount - 2;

            Prompts.DisplayIfVerbose(verboseMode, () =>
                Prompts.VerboseMessage(
                    $"[bold blue] (*)Calculated hosts per subnet: [/][green italic]{hostsPerSubnet}[/]."
                )
            );
            return new List<int>(new int[subnetCount].Fill(hostsPerSubnet));
        }

        public void DisplaySubnets(bool verboseMode = false)
        {
            Table table = AppUI.GenerateOuput();

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

            Prompts.DisplayIfVerbose(verboseMode, () =>
                Prompts.VerboseMessage(
                    $"\n\t[bold blue] (*) Output for all subnets generated in the table above...(*) [/]\n{new string('-', 100)}"
                )
            );
        }
    }
}
