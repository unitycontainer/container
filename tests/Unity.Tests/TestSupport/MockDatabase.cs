
namespace Unity.Tests.v5.TestSupport
{
    public class MockDatabase
    {
        private string connectionString;
        private bool defaultConstructorCalled;

        public MockDatabase()
        {
            defaultConstructorCalled = true;
        }

        public MockDatabase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static MockDatabase Create(string connectionString)
        {
            return new MockDatabase(connectionString);
        }

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public bool DefaultConstructorCalled
        {
            get { return defaultConstructorCalled; }
        }
    }
}
