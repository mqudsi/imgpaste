using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imgpaste
{
    class Program
    {
        const string UsageText = "USAGE: imgpaste.exe file1.png [file2.jpg ..]";
        const string VersionText = "imgpaste 0.1 by NeoSmart Technologies - https://neosmart.net/";

        static async Task<int> Main(string[] args)
        {
            var outputs = await ParseArgumentsAsync(args);

            try
            {
                var imgpaste = new ImagePaste();
                imgpaste.Capture();

                foreach (var dest in outputs)
                {
                    imgpaste.SaveAs(dest);
                }
            }
            catch (InvalidClipboardDataException ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return -2;
            }

            return 0;
        }

        private static async Task<List<string>> ParseArgumentsAsync(IReadOnlyList<string> args)
        {
            var outputs = new List<string>();

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (arg.ToLowerInvariant() == "--help" || arg == "-h")
                    {
                        await Console.Out.WriteLineAsync(UsageText);
                        await Console.Out.WriteLineAsync("Paste an image from the system clipboard to one or more named files.");
                        await Console.Out.WriteLineAsync();
                        await Console.Out.WriteLineAsync("Supported arguments:");
                        await Console.Out.WriteLineAsync("\t-h --help      Print this help information and exit");
                        await Console.Out.WriteLineAsync("\t-v --version   Print version information and exit");
                        Environment.Exit(0);
                    }
                    else if (arg.ToLowerInvariant() == "--version" || arg == "-V")
                    {
                        await Console.Out.WriteLineAsync(VersionText);
                        Environment.Exit(0);
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync("Invalid or unknown argument!");
                        await Console.Out.WriteLineAsync(UsageText);
                        await Console.Out.WriteLineAsync("See imgpaste --help for further usage information");
                        Environment.Exit(-1);
                    }
                }
                else
                {
                    outputs.Add(arg);
                }
            }

            if (outputs.Count == 0)
            {
                //no files provided!
                await Console.Error.WriteLineAsync(UsageText);
                Environment.Exit(-1);
            }

            return outputs;
        }
    }
}
