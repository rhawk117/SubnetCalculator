using Spectre.Console;
using System;

namespace SubnetCalculator
{
    public static class Prompts
    {
        public static void DisplayIfVerbose(bool verbose, Action verboseInfo)
        {
            if (verbose) verboseInfo.Invoke();
        }
        public static void Error(string errorInfo)
        {
            AnsiConsole.MarkupLine($"\n[bold red](!) {errorInfo} (!)[/]\n");
            AnsiConsole.MarkupLine($"\n\t[bold green](>) Press ENTER to Awknowledge and Continue... (<)[/]\n");
            Console.ReadKey();
        }

        public static void Question(string prompt) =>
            AnsiConsole.MarkupLine($"[bold blue](?) {prompt} (?)[/]");

        public static void VerboseMessage(string verboseInfo) =>
            AnsiConsole.MarkupLine(verboseInfo);

        public static void InfoMessage(string info) =>
            AnsiConsole.MarkupLine($"[bold yellow] (i) {info} (i) [/]");
    }
}
