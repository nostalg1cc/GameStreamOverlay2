using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace VideoOverlay
{
    public class WindowsServices
    {
        // Win32 constants
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_CLICKTHROUGH = WS_EX_TRANSPARENT | WS_EX_LAYERED;
        
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public void MakeWindowClickThrough(Window window)
        {
            // Get the window handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            
            // Get current style
            int style = GetWindowLong(hwnd, GWL_EXSTYLE);
            
            // Add click-through flag
            SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_CLICKTHROUGH);
        }

        public void MakeWindowClickable(Window window)
        {
            // Get the window handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            
            // Get current style
            int style = GetWindowLong(hwnd, GWL_EXSTYLE);
            
            // Remove click-through flag
            SetWindowLong(hwnd, GWL_EXSTYLE, style & ~WS_EX_TRANSPARENT);
        }
    }
}
