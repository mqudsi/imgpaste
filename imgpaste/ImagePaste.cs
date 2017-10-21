using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imgpaste
{
    class ImagePaste
    {
        Image _image = null;

        /// <summary>
        /// Must be called only after opening the clipboard with OpenClipboard(...)!
        /// </summary>
        /// <returns></returns>
        private List<Win32.ClipboardFormats> GetClipboardFormats()
        {
            List<Win32.ClipboardFormats> formats = new List<Win32.ClipboardFormats>();

            uint format = 0;
            while ((format = Win32.EnumClipboardFormats(format)) != 0)
            {
                formats.Add((Win32.ClipboardFormats)format);
            }

            return formats;
        }

        public void Capture()
        {
            //Clipboard.ContainsImage() is always returning false
            //we've verified clipboard contains CF_BITMAP, CF_DIB, and CF_DIBv5
            //Clipboard.GetDataObject() always returns null, too!

            if (!Win32.OpenClipboard(IntPtr.Zero))
            {
                throw new SystemException("OpenClipboard() failed!");
            }

            var clipboardFormats = GetClipboardFormats();
            if (!clipboardFormats.Contains(Win32.ClipboardFormats.CF_BITMAP))
            {
                throw new InvalidClipboardDataException();
            }

            IntPtr hBitmap = Win32.GetClipboardData((uint)Win32.ClipboardFormats.CF_BITMAP);
            _image = Image.FromHbitmap(hBitmap);

            if (!Win32.CloseClipboard())
            {
                throw new SystemException("CloseClipboard() failed!");
            }
        }

        public void SaveAs(string path)
        {
            if (_image == null)
            {
                throw new InvalidOperationException("No image has been captured! Use the Capture() method before calling SaveAs()!");
            }

            _image.Save(path);
        }
    }
}
