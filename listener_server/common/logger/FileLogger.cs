using System;
using System.IO;
using System.Text;

namespace listener_server.common.logger
{
    public class FileLogger
    {
        private LoggingLevels logLevel = LoggingLevels.ERROR;
        private FileInfo logFile;
        private string _filename;
        private string _baseFileName;

        public FileLogger(string filename)
        {
            _baseFileName = filename;
            GetFileName(filename);
            logFile = new FileInfo(_filename);
            if (!logFile.Exists)
            {
                CreateFile();
            }
        }

        public FileLogger(string filename, LoggingLevels logLevel)
            : this(filename)
        {
            this.logLevel = logLevel;
        }

        public LoggingLevels LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }

        protected FileInfo LogFile { get { return logFile; } }

        public virtual void CreateFile()
        {
            GetFileName(_baseFileName);
            logFile = new FileInfo(_filename);
            if (!logFile.Exists)
            {
                FileStream fileStream = logFile.Create();
                fileStream.Close();
            }

            LogInfo("Logging File Created");
            Log("Logging events to: " + logFile.FullName);
        }

        protected virtual void LogMessage(string message, LoggingLevels lvl)
        {
            //log all message equal or below the current log level
            // ALWAYS log level IGNORE messages
            if (lvl <= LogLevel || lvl.Equals(LoggingLevels.IGNORE))
            {

                //Prepare message to write
                StringBuilder sb = new StringBuilder();
                DateTime now = DateTime.Now;
                sb.AppendFormat("{0} {1} {2} - {3}: ", now.ToShortDateString(), now.ToLongTimeString(), now.Millisecond, lvl.ToString("g"));    //preamble
                sb.Append(message.Replace("\n", Environment.NewLine));

                if (AppConfigHelper.LogToConsole || lvl.Equals(LoggingLevels.IGNORE))
                {
                    var prev = Console.ForegroundColor;
                    switch (lvl)
                    {
                        case LoggingLevels.ERROR:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case LoggingLevels.WARNING:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case LoggingLevels.IGNORE:
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            break;
                        case LoggingLevels.INFO:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case LoggingLevels.DEBUG:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }

                    Console.WriteLine(sb.ToString());
                    Console.ForegroundColor = prev;

                }

                //Write message to file
                FileStream fs = null;
                try
                {
                    fs = logFile.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                }
                catch (FileNotFoundException)
                {
                    CreateFile();
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }
        }

        public void Log(string message)
        {
            LogMessage(message, LoggingLevels.IGNORE);
        }

        public void LogError(string message)
        {
            LogMessage(message, LoggingLevels.ERROR);
        }

        public void LogWarning(string message)
        {
            LogMessage(message, LoggingLevels.WARNING);
        }

        public void LogInfo(string message)
        {
            LogMessage(message, LoggingLevels.INFO);
        }

        public void LogDebug(string message)
        {
            LogMessage((AppConfigHelper.MaxDebugChars > 0 && message.Length > AppConfigHelper.MaxDebugChars
                                          ? message.Substring(0, AppConfigHelper.MaxDebugChars) + "... (trimmed to " + AppConfigHelper.MaxDebugChars + " characters)"
                                          : message), LoggingLevels.DEBUG);
        }

        private void GetFileName(string fileName)
        {
            string dateTimeString = DateTime.Now.ToShortDateString().Replace("/", string.Empty);
            _filename = string.Concat(Path.GetDirectoryName(fileName), "\\", Path.GetFileNameWithoutExtension(fileName), "_", dateTimeString, Path.GetExtension(fileName));

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(fileName))) Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            catch (DirectoryNotFoundException)
            {
                _filename = "C" + _filename.Substring(1);
                if (!Directory.Exists(Path.GetDirectoryName(_filename))) Directory.CreateDirectory(Path.GetDirectoryName(_filename));
                logFile = new FileInfo(_filename);
            }
            catch (IOException)
            {
                _filename = "C" + _filename.Substring(1);
                if (!Directory.Exists(Path.GetDirectoryName(_filename))) Directory.CreateDirectory(Path.GetDirectoryName(_filename));
                logFile = new FileInfo(_filename);
            }
        }
    }
}
