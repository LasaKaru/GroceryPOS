using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Utilities.Logging
{
    public static class AppLogger
    {
        private static readonly string LogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
        private static readonly string LogFileName = $"AppLog_{DateTime.Now:yyyyMMdd}.txt";
        private static readonly object _lock = new object(); // To prevent concurrent writes

        static AppLogger()
        {
            try
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
            }
            catch (Exception ex)
            {
                // If we can't create the log directory, we'll log to debug output as a fallback.
                System.Diagnostics.Debug.WriteLine($"ERROR: Could not create log directory '{LogDirectory}'. Logging to debug output. Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes an informational message to the log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// Writes a warning message to the log file.
        /// </summary>
        /// <param name="message">The warning message.</param>
        public static void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        /// <summary>
        /// Writes an error message to the log file, optionally including an exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">Optional: The exception that occurred.</param>
        public static void LogError(string message, Exception? ex = null)
        {
            string logMessage = message;
            if (ex != null)
            {
                logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
                if (ex.InnerException != null)
                {
                    logMessage += $"\nInner Exception: {ex.InnerException.Message}";
                }
            }
            WriteLog("ERROR", logMessage);
        }

        private static void WriteLog(string level, string message)
        {
            string logFilePath = Path.Combine(LogDirectory, LogFileName);
            string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";

            lock (_lock) // Ensure only one write operation happens at a time
            {
                try
                {
                    File.AppendAllText(logFilePath, formattedMessage + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // Fallback: If writing to file fails, write to Debug output
                    System.Diagnostics.Debug.WriteLine($"FATAL LOGGING ERROR: Could not write to log file '{logFilePath}'. Message: {formattedMessage}. Exception: {ex.Message}");
                }
            }
        }
    }
}
