using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubnetCalculator
{
    public static class SubnetUtils
    {
        public static (IPAddress ip, int prefixLength) ParseCidrNotation(string cidrNotation, bool verboseMode = false)
        {
            string[] parts = cidrNotation.Split('/');
            if (verboseMode)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Parsed CIDR Notation:[/] IP Address: [green]{parts[0]}[/], Prefix Length: [green]{parts[1]}[/]");
            }
            return (IPAddress.Parse(parts[0]), int.Parse(parts[1]));
        }

        public static uint IpToUint(IPAddress ip, bool verboseMode = false)
        {
            uint ipAsUint = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
            if (verboseMode)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Converted IP Address to Uint:[/] [green]{ipAsUint}[/]");
            }
            return ipAsUint;
        }

        public static (uint subnetMask, uint subnetIncrement) CalculateSubnetMaskAndIncrement(int hostCount, bool verboseMode = false)
        {
            int subnetBits = (int)Math.Ceiling(Math.Log(hostCount + 2, 2));
            int newPrefixLength = 32 - subnetBits;

            uint subnetMask = 0xFFFFFFFF << (32 - newPrefixLength);
            uint subnetIncrement = 1u << (32 - newPrefixLength);

            if (verboseMode)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Calculated Subnet Mask:[/] [green]{new IPAddress(BitConverter.GetBytes(subnetMask).Reverse().ToArray())}[/]");
                AnsiConsole.MarkupLine($"[bold yellow]Subnet Increment:[/] [green]{subnetIncrement}[/]");
            }

            return (subnetMask, subnetIncrement);
        }

        public static T[] Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
            return array;
        }
    }
}

