using System.Runtime.InteropServices;
using System.Windows;

namespace Helper
{
    public static class CursorController
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out PointInter lpPoint);

        /// <summary>
        ///     Получение положения курсора относительно экрана
        /// </summary>
        /// <returns></returns>
        public static Point GetCursorPosition()
        {
            GetCursorPos(out var lpPoint);
            return (Point) lpPoint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;

            public static explicit operator Point(PointInter point)
            {
                return new Point(point.X, point.Y);
            }
        }
    }
}