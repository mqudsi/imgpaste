using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clipboard = imgpaste.Win32Clipboard;

namespace imgpaste
{
    class ImagePaste : IDisposable
    {
        Image _image = null;
        static readonly ImageFormat DefaultFormat = ImageFormat.Png;
        static readonly Dictionary<string, ImageFormat> FormatDictionary = new Dictionary<string, ImageFormat>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".bmp", ImageFormat.Bmp },
            { ".gif", ImageFormat.Gif },
            { ".jpeg", ImageFormat.Jpeg },
            { ".jpg", ImageFormat.Jpeg },
            { ".png", ImageFormat.Png },
        };

        public void Capture()
        {
            //The "real"/system Clipboard.ContainsImage() is always returning false
            //We've verified clipboard contains CF_BITMAP, CF_DIB, and CF_DIBv5
            //and Clipboard.GetDataObject() always returns null, too!
            //See https://www.youtube.com/watch?v=HkIuXQytmOA for reference

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

            var imgFormat = SelectFormat(path);
            _image.Save(path, imgFormat);
        }

        private ImageFormat SelectFormat(string filename)
        {
            var ext = Path.GetExtension(filename);

            if (!string.IsNullOrWhiteSpace(ext))
            {
                if (FormatDictionary.TryGetValue(ext, out var format))
                {
                    return format;
                }
            }

            return DefaultFormat;
        }

        public void Dispose()
        {
            _image?.Dispose();
            _image = null;
        }
    }
}
