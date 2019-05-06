namespace Unity.Tests.TestSupport
{
    public class ObjectUsingLogger
    {
        private ILogger logger;

        [Dependency]
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
    }
}
