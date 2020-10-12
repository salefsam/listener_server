using System;

namespace listener_server.common.logger
{
    public class Logger
    {
        private static readonly SharedFileLogger Log = new SharedFileLogger(AppConfigHelper.LogFilePath, AppConfigHelper.LoggingLevel);
        public static LoggingLevels LoggingLevel { get { return Log.LogLevel; } }


        /*
         * Method to generate a new log file if necessary.
         */
        // TODO I dont really like this, but as Listener can run for days, need a way to generate new log file per day
        public static void checkFile()
        {
            Log.CreateFile();
        }

        // Log messages at the IGNORE level setting should always display be logged regardless of logging level
        public static void Ignore(string msg)
        {
            try
            {
                Log.Log(msg);
            }
            catch (Exception e)
            {
                //ignore.
                Console.Write(e.StackTrace);
            }
        }

        public static void Debug(string msg)
        {
            try
            {
                string dmsg = (AppConfigHelper.MaxDebugChars > 0 && msg.Length > AppConfigHelper.MaxDebugChars
                    ? msg.Substring(0, AppConfigHelper.MaxDebugChars) + "... (trimmed)"
                    : msg);
                Log.LogDebug(dmsg);
            }
            catch (Exception e)
            {
                //ignore.
                Console.Write(e.StackTrace);
            }
        }

        public static void Info(string msg)
        {
            try
            {
                Log.LogInfo(msg);
            }
            catch (Exception e)
            {
                //ignore
                Console.Write(e.StackTrace);
            }
        }

        public static void Warning(string msg)
        {
            try
            {
                Log.LogWarning(msg);
            }
            catch (Exception e)
            {
                //ignore
                Console.Write(e.StackTrace);
            }
        }

        public static string ErrorMessage(Exception e)
        {
            String ErrMessage = String.Concat(Environment.NewLine, "\t", e.Message, Environment.NewLine,
                            "\t", "--------BEGIN STACK TRACE--------", Environment.NewLine,
                            "\t", e.StackTrace, Environment.NewLine,
                            "\t", "--------END STACK TRACE----------");

            if (e.InnerException != null)
            {
                ErrMessage = String.Concat(ErrMessage, "Inner Exception Found:\r\n" + ErrorMessage(e.InnerException));
            }

            return ErrMessage;
        }

        public static void Error(string msg)
        {
            try
            {
                Log.LogError(msg);
            }
            catch (Exception e)
            {
                //ignore
                Console.Write(e.StackTrace);
            }
        }
    }
}