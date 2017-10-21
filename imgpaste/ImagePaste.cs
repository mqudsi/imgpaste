using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clipboard = imgpaste.Win32Clipboard;

namespace imgpaste
{
    class ImagePaste : IDisposable
    {
        Image _image = null;

        public void Capture()
        {
            //Clipboard.ContainsImage() is always returning false
            //we've verified clipboard contains CF_BITMAP, CF_DIB, and CF_DIBv5
            //Clipboard.GetDataObject() always returns null, too!

            using (var clipboard = new Clipboard())
            {
                var clipboardFormats = clipboard.GetFormats();
                if (!clipboardFormats.Contains(Clipboard.Format.CF_BITMAP))
                {
                    throw new InvalidClipboardDataException();
                }

                var hBitmap = clipboard.GetData(Clipboard.Format.CF_BITMAP);
                _image = Image.FromHbitmap(hBitmap);
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

        public void Dispose()
        {
            _image?.Dispose();
            _image = null;
        }
    }
}
