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
            if (args.Length == 0)
            {
                await Console.Error.WriteLineAsync("USAGE: imgpaste.exe screenshot.png [file2.jpg ..]");
                return -1;
            }

            try
            {
                var imgpaste = new ImagePaste();
                imgpaste.Capture();

                foreach (var dest in args)
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
    }
}
