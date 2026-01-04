using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SuppressImportWarnings
{
    private static ILogHandler defaultLogHandler;

    static SuppressImportWarnings()
    {
        // Store the default log handler
        defaultLogHandler = Debug.unityLogger.logHandler;
        
        // Set our custom log handler that filters out import warnings
        Debug.unityLogger.logHandler = new FilteredLogHandler(defaultLogHandler);
    }

    private class FilteredLogHandler : ILogHandler
    {
        private ILogHandler defaultHandler;

        public FilteredLogHandler(ILogHandler defaultHandler)
        {
            this.defaultHandler = defaultHandler;
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            string message = string.Format(format, args);
            
            // Filter out identifier uniqueness violation warnings
            if (logType == LogType.Warning && 
                message.Contains("Identifier uniqueness violation") &&
                message.Contains("Multiple Objects with the same name/type are generated"))
            {
                // Suppress this warning - don't log it
                return;
            }

            // Log all other messages normally
            defaultHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(System.Exception exception, Object context)
        {
            // Filter out DirectoryNotFoundException from Synty asset trying to write to non-existent paths
            if (exception is System.IO.DirectoryNotFoundException)
            {
                string exceptionMessage = exception.ToString();
                if (exceptionMessage.Contains("version.txt") && 
                    (exceptionMessage.Contains("Synty") || exceptionMessage.Contains("SidekickCharacters")))
                {
                    // Suppress this exception - don't log it
                    return;
                }
            }

            // Log all other exceptions normally
            defaultHandler.LogException(exception, context);
        }
    }
}

