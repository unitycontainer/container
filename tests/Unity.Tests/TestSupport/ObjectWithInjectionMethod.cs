namespace Unity.Tests.v5.TestSupport
{
    public class ObjectWithInjectionMethod
    {
        public string ConnectionString { get; private set; }

        public ILogger Logger { get; private set; }

        public object MoreData { get; private set; }

        public void Initialize(string connectionString, ILogger logger)
        {
            ConnectionString = connectionString;
            Logger = logger;
        }

        public void MoreInitialization(object data)
        {
            MoreData = data;
        }
    }
}
