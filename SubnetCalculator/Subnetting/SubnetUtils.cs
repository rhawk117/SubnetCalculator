using System;
using System.Linq;
using System.Net;

namespace SubnetCalculator
{
    public static class SubnetUtils
    {
        public static (IPAddress ip, int prefixLength) ParseCIDR(string cidrNotation, bool verboseMode = false)
        {
            string[] parts = cidrNotation.Split('/');

            Prompts.DisplayIfVerbose(verboseMode, () =>
                Prompts.VerboseMessage($"Parsed CIDR Notation\n IP Address: [italic]{parts[0]}[/]\nPrefix Length: [italic]{parts[1]}\n[/]")
            );

            return (IPAddress.Parse(parts[0]), int.Parse(parts[1]));
        }

        public static uint IpToUint(IPAddress ip, bool verboseMode = false)
        {
            uint ipAsUint = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
            Prompts.DisplayIfVerbose(
                verboseMode, () =>
                Prompts.VerboseMessage($"[bold blue] (*) Converted IP Address to Uint: [/] [italic] {ipAsUint} [/]")
            );

            return ipAsUint;
        }

        public static (uint subnetMask, uint subnetIncrement) CalcSubnetMask(int hostCount,
        bool verboseMode = false)
        {
            int subnetBits = (int)Math.Ceiling(Math.Log(hostCount + 2, 2));
            int newPrefixLength = 32 - subnetBits;

            uint subnetMask = 0xFFFFFFFF << (32 - newPrefixLength);
            uint subnetIncrement = 1u << (32 - newPrefixLength);

            Prompts.DisplayIfVerbose(verboseMode, () =>
                displaySubnetCalc(subnetIncrement, subnetMask)
             );

            return (subnetMask, subnetIncrement);
        }

        private static void displaySubnetCalc(uint subnetIncrement, uint subnetMask)
        {
            Prompts.VerboseMessage($"[bold blue] (*) Calculated Subnet Mask: [/][italic]{new IPAddress(BitConverter.GetBytes(subnetMask).Reverse().ToArray())}[/]");
            Prompts.VerboseMessage($"[bold blue] (*) Subnet Increment: [/][italic]{subnetIncrement}[/]");
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

