using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imgpaste
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length != 1)
            {
                await Console.Error.WriteLineAsync("USAGE: imgpaste.exe screenshot.png");
                return -1;
            }

            try
            {
                var imgpaste = new ImagePaste();
                imgpaste.Capture();
                imgpaste.SaveAs(args[0]);
            }
            catch (InvalidClipboardDataException ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return -2;
            }

            return 0;
        }
    }
}
