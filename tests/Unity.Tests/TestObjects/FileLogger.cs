

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestObjects
{
    public class FileLogger
    {
        private string logFile;

        public FileLogger(string logFile)
        {
            this.logFile = logFile;
        }

        public string LogFile
        {
            get { return logFile; }
        }
    }
}
