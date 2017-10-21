using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imgpaste
{
    class Win32Clipboard : IDisposable
    {
        internal enum Format
        {
            CF_TEXT = 1,
            CF_BITMAP = 2,
            CF_METAFILEPICT = 3,
            CF_SYLK = 4,
            CF_DIF = 5,
            CF_TIFF = 6,
            CF_OEMTEXT = 7,
            CF_DIB = 8,
            CF_PALETTE = 9,
            CF_PENDATA = 10,
            CF_RIFF = 11,
            CF_WAVE = 12,
            CF_UNICODETEXT = 13,
            CF_ENHMETAFILE = 14,
            CF_HDROP = 15,
            CF_LOCALE = 16,
            CF_DIBV5 = 17,
        }

        public Win32Clipboard()
        {
            if (!Win32.OpenClipboard(IntPtr.Zero))
            {
                throw new SystemException("OpenClipboard() failed!");
            }
        }

        /// <summary>
        /// Must be called only after opening the clipboard with OpenClipboard(...)!
        /// </summary>
        /// <returns></returns>
        public List<Format> GetFormats()
        {
            List<Format> formats = new List<Format>();

            uint format = 0;
            while ((format = Win32.EnumClipboardFormats(format)) != 0)
            {
                formats.Add((Format)format);
            }

            return formats;
        }

        public IntPtr GetData(Format format)
        {
            return Win32.GetClipboardData((uint) format);
        }

        public void Dispose()
        {
            if (!Win32.CloseClipboard())
            {
                throw new SystemException("CloseClipboard() failed!");
            }
        }
    }
}
