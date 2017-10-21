using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imgpaste
{
    class InvalidClipboardDataException : Exception
    {
        public InvalidClipboardDataException(string message, Exception innerException = null)
            : base(message, innerException)
        { }

        public InvalidClipboardDataException()
            : this("The clipboard does not contain valid bitmap data!", null)
        { }
    }
}
