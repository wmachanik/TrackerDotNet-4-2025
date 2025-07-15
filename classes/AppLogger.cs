using System;
using System.IO;
using System.Linq;
using System.Web;

namespace TrackerDotNet.Classes
{
    /// <summary>
    /// Provides centralized logging to App_Data with log rotation and named log files.
    /// </summary>
    public static class AppLogger
    {
        private const int MaxLines = 5000;

        /// <summary>
        /// Writes a log entry to App_Data under a specified log name.
        /// Automatically trims file if size exceeds MaxLines.
        /// </summary>
        /// <param name="logName">Name of the log file, without extension (e.g., "email", "diagnostics").</param>
        /// <param name="message">The log message to write.</param>
        public static void WriteLog(string logName, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(logName))
                    logName = "general";

                string logDir;

                if (HttpContext.Current != null && HttpContext.Current.Server != null)
                {
                    logDir = HttpContext.Current.Server.MapPath("~/App_Data/");
                }
                else
                {
                    // Fallback for non-HTTP contexts — logs to app base directory
                    logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

                    // Ensure the folder exists
                    if (!Directory.Exists(logDir))
                        Directory.CreateDirectory(logDir);
                }

                string filePath = Path.Combine(logDir, $"{logName}.log");
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string entry = $"[{timeStamp}] {message}";

                File.AppendAllText(filePath, entry + Environment.NewLine);
                TrimLogFile(filePath);
            }
            catch
            {
                // Optional: write to Windows Event Log, Debug, or ignore silently
            }
        }

        /// <summary>
        /// Trims the log file to retain only the most recent MaxLines.
        /// </summary>
        private static void TrimLogFile(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length > MaxLines)
                {
                    var trimmed = lines.Skip(lines.Length - MaxLines).ToArray();
                    File.WriteAllLines(filePath, trimmed);
                }
            }
            catch
            {
                // Optional: suppress trim failure
            }
        }
    }
}
