using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace PortraitClip
{
    public static class ScreenManager
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref POINT p);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        public static Point GetCursorPosition()
        {
            var p = new POINT();
            GetCursorPos(ref p);
            return new Point(p.x, p.y);
        }

        public static void SetCursorPosition(Point p)
        {
            SetCursorPos((int)p.X, (int)p.Y);
        }

        [DebuggerDisplay(@"\{{x}, {y}\}")]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
    }
}
