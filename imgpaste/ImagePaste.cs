using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using Clipboard = imgpaste.Win32Clipboard;
using PixelFormat = SixLabors.ImageSharp.Rgba32;

namespace imgpaste
{
    class ImagePaste : IDisposable
    {
        static class Encoders
        {
            public static IImageEncoder Bmp => new BmpEncoder();
            public static IImageEncoder Gif => new GifEncoder();
            public static IImageEncoder Jpeg => new JpegEncoder()
            {
                Quality = 90
            };
            public static IImageEncoder Png => new PngEncoder()
            {
                CompressionLevel = 9,
                PngColorType = PngColorType.Palette,
            };
        }
        SixLabors.ImageSharp.Image<PixelFormat> _image = null;
        static readonly IImageFormat DefaultFormat = ImageFormats.Png;
        static readonly Dictionary<string, Func<IImageEncoder>> EncoderDictionary = new Dictionary<string, Func<IImageEncoder>>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".bmp", () => Encoders.Bmp },
            { ".gif", () => Encoders.Gif },
            { ".jpeg", () => Encoders.Jpeg },
            { ".jpg", () => Encoders.Jpeg },
            { ".png", () => Encoders.Png },
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
                using (var sdImage = System.Drawing.Image.FromHbitmap(hBitmap))
                using (var mstream = new MemoryStream())
                {
                    sdImage.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);
                    mstream.Seek(0, SeekOrigin.Begin);
                    _image = Image.Load<PixelFormat>(mstream);
                }
            }
        }

        public void SaveAs(string path)
        {
            if (_image == null)
            {
                throw new InvalidOperationException("No image has been captured! Use the Capture() method before calling SaveAs()!");
            }

            var encoder = SelectEncoder(path);

            using (var outFile = System.IO.File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                _image.Save(outFile, encoder);
            }
        }

        private IImageEncoder SelectEncoder(string filename)
        {
            var ext = Path.GetExtension(filename);

            if (!string.IsNullOrWhiteSpace(ext))
            {
                if (EncoderDictionary.TryGetValue(ext, out var encoderGenerator))
                {
                    return encoderGenerator();
                }
            }

            return Encoders.Png;
        }

        public void Dispose()
        {
            _image?.Dispose();
            _image = null;
        }
    }
}
