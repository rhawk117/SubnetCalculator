using Spectre.Console;
using System;
using System.Linq;
using System.Net;

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

            Prompts.DisplayIfVerbose(verboseMode, () =>
                VerboseSubnetInfo()
            );
        }

        private void VerboseSubnetInfo()
        {
            Console.WriteLine(new string('=', 100));
            AnsiConsole.MarkupLine($"[bold blue](*) Subnet Created (*) [/]");
            AnsiConsole.MarkupLine($"[bold blue]Network ID:[/] [italic green]{NetworkId}[/]");
            AnsiConsole.MarkupLine($"[bold blue]Subnet Mask:[/] [italic green]{SubnetMask}[/]");
            AnsiConsole.MarkupLine($"[bold blue]First Host:[/] [italic green]{FirstHost}[/]");
            AnsiConsole.MarkupLine($"[bold blue]Last Host:[/] [italic green]{LastHost}[/]");
            AnsiConsole.MarkupLine($"[bold blue]Broadcast Address:[/] [italic green]{Broadcast}[/]");
            Console.WriteLine(new string('=', 100));

            AnsiConsole.MarkupLine("\n\t(*) Press [bold green]ENTER[/] to Continue...(*)");
            Console.ReadKey();
        }
    }

}
