using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace SharedLogic
{
    [PublicAPI]
    public class Logger
    {
        public string Namespace { get; }
        
        public Logger(string @namespace)
        {
            Namespace = @namespace;
        }
        
        [PublicAPI] public static bool ShowTime { get; set; }
        
        [PublicAPI] public static bool ShowDebug { get; set; } = true;

        [PublicAPI] public static bool Enabled { get; set; } = true;

        [PublicAPI] public static bool UseColor { get; set; } = true;

        public void Log(Color foreground, Color background, 
                        Color categoryForeground, Color categoryBackground,
                        bool newLine, bool stderr,
                        string category, string message,
                        params object[] arg)
        {
            if (!Enabled) return;

            var writer = stderr ? Console.Error : Console.Out;
            
            if (UseColor && !EnableVT())
            {
                UseColor = false;
            }

            if (UseColor && categoryForeground != default)
            {
                writer.Write(categoryForeground.ToForeground());
            }
            
            if (UseColor && categoryBackground != default)
            {
                writer.Write(categoryBackground.ToBackground());
            }

            if (ShowTime)
            {
                writer.Write($"[{DateTimeOffset.UtcNow:O}]");
            }
            
            if (!string.IsNullOrWhiteSpace(category))
            {
                writer.Write($"[{category}]");
            }

            writer.Write($"[{Namespace}]");
            
            if (UseColor && foreground != categoryForeground) writer.Write(ColorReset);
            
            var output = message;

            if (arg.Length > 0) output = string.Format(message, arg);

            if (output.Contains("\n")) writer.WriteLine();
            else writer.Write(" ");
            
            if (UseColor && foreground != default)
            {
                writer.Write(foreground.ToForeground());
            }
            
            if (UseColor && background != default)
            {
                writer.Write(background.ToBackground());
            }
            
            writer.Write(output);

            if (UseColor) writer.Write(ColorReset);

            if (newLine) writer.WriteLine();
        }

        public void Success(string category, string message, params object[] arg)
        {
            Log( Color.White, Color.Black, Color.SeaGreen, Color.Black, true, false, category ?? "S", message, arg);
        }

        public void Info(string category, string message, params object[] arg)
        {
            Log( Color.White, Color.Black, Color.Azure, Color.Black, true, false, category ?? "I", message, arg);
        }

        public void Debug(string category, string message, params object[] arg)
        {
            Log( Color.White, Color.Black, Color.DimGray, Color.Black, true, false, category ?? "D", message, arg);
        }

        public void Warn(string category, string message, params object[] arg)
        {
            Log( Color.White, Color.Black, Color.Yellow, Color.Black, true, true, category ?? "W", message, arg);
        }

        public void Error(string category, string message, params object[] arg)
        {
            Log( Color.White, Color.Black, Color.Red, Color.Black, true, true, category ?? "E", message, arg);
        }

        public void Fatal(string category, string message, params object[] arg)
        {
            Log( Color.White, Color.Red, Color.White, Color.Red, true, true, category ?? "!", message, arg);
        }

        public void ResetColor(TextWriter writer)
        {
            if (!UseColor)
            {
                return;
            }
            
            if (!EnableVT())
                Console.ResetColor();
            else
                writer.Write(ColorReset);
        }
        
        public const string ColorReset = "\x1b[0m";
        
        private const int STD_OUTPUT_HANDLE = -11;
        private const int STD_ERROR_HANDLE = -12;
        private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public static bool IsVTEnabled { get; private set; }
        public static bool IsVTCapable { get; private set; } = Environment.OSVersion.Version.Major >= 6;

        public static bool EnableVT()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return true; // always on with unix.

            if (IsVTEnabled) return true;

            if (!IsVTCapable) return false;

            if (Environment.OSVersion.Platform != PlatformID.Win32NT || !Console.IsOutputRedirected)
            {
                if (!EnableColor(STD_OUTPUT_HANDLE))
                    return false;
            }

            // ReSharper disable once InvertIf
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || !Console.IsErrorRedirected)
            {
                if (!EnableColor(STD_ERROR_HANDLE))
                    return false;
            }

            return true;
        }

        private static unsafe bool EnableColor(int stdHandle)
        {
            var hOut = GetStdHandle(stdHandle);
            if (hOut == INVALID_HANDLE_VALUE)
            {
                IsVTCapable = false;
                return false;
            }

            var dwMode = 0;
            if (!GetConsoleMode(hOut, &dwMode))
            {
                IsVTCapable = false;
                return false;
            }

            dwMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            if (!SetConsoleMode(hOut, dwMode))
            {
                IsVTCapable = false;
                return false;
            }

            IsVTEnabled = true;
            return true;
        }

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("Kernel32.dll")]
        private static extern unsafe bool GetConsoleMode(IntPtr hConsoleHandle, int* lpMode);

        [DllImport("Kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);
    }
}
