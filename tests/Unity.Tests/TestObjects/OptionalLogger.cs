
using Unity.Tests.v5.TestDoubles;

namespace Unity.Tests.v5.TestObjects
{
    internal class OptionalLogger
    {
        private string logFile;

        public OptionalLogger([Dependency] string logFile)
        {
            this.logFile = logFile;
        }

        public string LogFile
        {
            get { return logFile; }
        }
    }
}
