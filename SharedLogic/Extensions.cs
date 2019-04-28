using System.Drawing;

namespace SharedLogic
{
    public static class Extensions
    {
        public const string ColorReset = "\x1b[0m";
        
        public static string ToForeground(this Color color)
        {
            return color.A == 0 ? ColorReset : $"\x1b[38;2;{color.R};{color.G};{color.B}m";
        }

        public static string ToBackground(this Color color)
        {
            return color.A == 0 ? ColorReset : $"\x1b[48;2;{color.R};{color.G};{color.B}m";
        }
    }
}
