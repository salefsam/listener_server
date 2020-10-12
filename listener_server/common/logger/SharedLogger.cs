namespace listener_server.common.logger
{
    public class SharedFileLogger : FileLogger
    {
        public SharedFileLogger(string filename) : base(filename)
        {

        }

        public SharedFileLogger(string filename, LoggingLevels lvl) : base(filename, lvl)
        {

        }

        protected override void LogMessage(string message, LoggingLevels lvl)
        {
            lock(LogFile)
            {
                base.LogMessage(message, lvl);
            }
        }

        public override void CreateFile()
        {
            lock(LogFile)
            {
                base.CreateFile();
            }
        }
    }
}