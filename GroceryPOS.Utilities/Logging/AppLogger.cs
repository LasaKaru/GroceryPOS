using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Utilities.Logging
{
    //public static class AppLogger
    //{
    //    private static readonly string LogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
    //    private static readonly string LogFileName = $"AppLog_{DateTime.Now:yyyyMMdd}.txt";
    //    private static readonly object _lock = new object(); // To prevent concurrent writes

    //    static AppLogger()
    //    {
    //        try
    //        {
    //            if (!Directory.Exists(LogDirectory))
    //            {
    //                Directory.CreateDirectory(LogDirectory);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // If we can't create the log directory, we'll log to debug output as a fallback.
    //            System.Diagnostics.Debug.WriteLine($"ERROR: Could not create log directory '{LogDirectory}'. Logging to debug output. Exception: {ex.Message}");
    //        }
    //    }

    //    /// <summary>
    //    /// Writes an informational message to the log file.
    //    /// </summary>
    //    /// <param name="message">The message to log.</param>
    //    public static void LogInfo(string message)
    //    {
    //        WriteLog("INFO", message);
    //    }

    //    /// <summary>
    //    /// Writes a warning message to the log file.
    //    /// </summary>
    //    /// <param name="message">The warning message.</param>
    //    public static void LogWarning(string message)
    //    {
    //        WriteLog("WARNING", message);
    //    }

    //    /// <summary>
    //    /// Writes an error message to the log file, optionally including an exception.
    //    /// </summary>
    //    /// <param name="message">The error message.</param>
    //    /// <param name="ex">Optional: The exception that occurred.</param>
    //    public static void LogError(string message, Exception? ex = null)
    //    {
    //        string logMessage = message;
    //        if (ex != null)
    //        {
    //            logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
    //            if (ex.InnerException != null)
    //            {
    //                logMessage += $"\nInner Exception: {ex.InnerException.Message}";
    //            }
    //        }
    //        WriteLog("ERROR", logMessage);
    //    }

    //    private static void WriteLog(string level, string message)
    //    {
    //        string logFilePath = Path.Combine(LogDirectory, LogFileName);
    //        string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";

    //        lock (_lock) // Ensure only one write operation happens at a time
    //        {
    //            try
    //            {
    //                File.AppendAllText(logFilePath, formattedMessage + Environment.NewLine);
    //            }
    //            catch (Exception ex)
    //            {
    //                // Fallback: If writing to file fails, write to Debug output
    //                System.Diagnostics.Debug.WriteLine($"FATAL LOGGING ERROR: Could not write to log file '{logFilePath}'. Message: {formattedMessage}. Exception: {ex.Message}");
    //            }
    //        }
    //    }
    //}
    public static class AppLogger
    {
        private static readonly string LogDirectory;
        private static readonly string LogFilePath;
        private static readonly object FileLock = new object();

        static AppLogger()
        {
            // Determine the base application directory (e.g., where the executable is located)
            string? currentAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(currentAssemblyPath))
            {
                currentAssemblyPath = AppDomain.CurrentDomain.BaseDirectory; // Fallback
            }

            // Define a logs subdirectory within the application's local data folder
            // This will be more appropriate for a deployed app
            LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GroceryPOS", "Logs");
            LogFilePath = Path.Combine(LogDirectory, $"GroceryPOS_Log_{DateTime.Today:yyyy-MM-dd}.log");

            // Ensure the log directory exists
            try
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
            }
            catch (Exception ex)
            {
                // Fallback to console/debug output if log directory creation fails
                Debug.WriteLine($"CRITICAL ERROR: Failed to create log directory: {LogDirectory}. {ex.Message}");
                LogDirectory = Path.GetTempPath(); // Fallback to temp path
                LogFilePath = Path.Combine(LogDirectory, $"GroceryPOS_Log_Fallback_{DateTime.Today:yyyy-MM-dd}.log");
            }
        }

        private static void WriteLog(string level, string message, Exception? ex = null)
        {
            // Format: [Timestamp] [Level] Message (Exception details)
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";
            if (ex != null)
            {
                logEntry += Environment.NewLine + $"Exception: {ex.GetType().Name} - {ex.Message}";
                logEntry += Environment.NewLine + $"StackTrace: {ex.StackTrace}";
                if (ex.InnerException != null)
                {
                    logEntry += Environment.NewLine + $"Inner Exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}";
                    logEntry += Environment.NewLine + $"Inner StackTrace: {ex.InnerException.StackTrace}";
                }
            }

            // Write to file with a lock to prevent concurrent access issues
            lock (FileLock)
            {
                try
                {
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception fileEx)
                {
                    // Fallback to debug output if writing to file fails
                    Debug.WriteLine($"ERROR: Failed to write to log file: {LogFilePath}. {fileEx.Message}");
                    Debug.WriteLine($"Original Log Entry: {logEntry}");
                }
            }

            // Also output to Debug console for immediate visibility during development
            Debug.WriteLine(logEntry);
        }

        public static void LogInfo(string message) => WriteLog("INFO", message);
        public static void LogWarning(string message) => WriteLog("WARN", message);
        public static void LogError(string message, Exception? ex = null) => WriteLog("ERROR", message, ex);
        public static void LogFatal(string message, Exception? ex = null) => WriteLog("FATAL", message, ex);
    }
}
