using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace imgpaste
{
    static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint EnumClipboardFormats(uint format);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool CloseClipboard();
    }
}
