using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Helper
{
    public static class WindowController
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        ///     Вывод окна на передний план
        /// </summary>
        public static void BringWindowToFront()
        {
            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
        }
    }
}