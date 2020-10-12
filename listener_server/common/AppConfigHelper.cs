using System;
using System.Configuration;
using listener_server.common.logger;

namespace listener_server.common
{
    class AppConfigHelper
    {
        public static string LogFilePath { get { return ConfigurationManager.AppSettings["log_file_path"]; } }

        // Getter for log to console flag
        public static bool LogToConsole { get { return Boolean.Parse(ConfigurationManager.AppSettings["log_to_console"]); } }

        // Getter for current log level
        public static LoggingLevels LoggingLevel
        {
            get
            {
                string tmp = ConfigurationManager.AppSettings["logging_level"];
                if ("DEBUG".Equals(tmp))
                {
                    return LoggingLevels.DEBUG;
                }
                if ("INFO".Equals(tmp))
                {
                    return LoggingLevels.INFO;
                }
                return "WARN".Equals(tmp) ? LoggingLevels.WARNING : LoggingLevels.ERROR;
            }
        }

        // Used to restrict the number of characters displayed in debug messages. Primarily used when dumping message traffic to screen.
        public static int MaxDebugChars
        {
            get
            {
                if (Convert.ToInt32(ConfigurationManager.AppSettings["max_debug_chars"]) > 0)
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["max_debug_chars"]);
                }
                return -1;
            }
        }
    }
}