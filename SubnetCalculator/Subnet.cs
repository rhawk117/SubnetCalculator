using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubnetCalculator
{
    public class Subnet
    {
        public IPAddress NetworkId { get; }
        public IPAddress SubnetMask { get; }
        public IPAddress FirstHost { get; }
        public IPAddress LastHost { get; }
        public IPAddress Broadcast { get; }

        public Subnet(uint baseIp, uint subnetMask, uint subnetIncrement, bool verboseMode = false)
        {
            NetworkId = new IPAddress(BitConverter.GetBytes(baseIp).Reverse().ToArray());
            SubnetMask = new IPAddress(BitConverter.GetBytes(subnetMask).Reverse().ToArray());

            uint firstHost = baseIp + 1;
            uint lastHost = baseIp + subnetIncrement - 2;
            uint broadcast = baseIp + subnetIncrement - 1;

            FirstHost = new IPAddress(BitConverter.GetBytes(firstHost).Reverse().ToArray());
            LastHost = new IPAddress(BitConverter.GetBytes(lastHost).Reverse().ToArray());
            Broadcast = new IPAddress(BitConverter.GetBytes(broadcast).Reverse().ToArray());

            if (verboseMode)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Subnet created with:[/]");
                AnsiConsole.MarkupLine($"[bold yellow]Network ID:[/] [green]{NetworkId}[/]");
                AnsiConsole.MarkupLine($"[bold yellow]Subnet Mask:[/] [green]{SubnetMask}[/]");
                AnsiConsole.MarkupLine($"[bold yellow]First Host:[/] [green]{FirstHost}[/]");
                AnsiConsole.MarkupLine($"[bold yellow]Last Host:[/] [green]{LastHost}[/]");
                AnsiConsole.MarkupLine($"[bold yellow]Broadcast Address:[/] [green]{Broadcast}[/]");
            }
        }
    }

}
