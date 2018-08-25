namespace Unity.Tests.v5.Override
{
    public class TestTypeInConfig
    {
        public TestTypeInConfig(int value)
        {
            Value = value;
        }

        public TestTypeInConfig()
        {
            Value = 1;
        }

        public int Value { get; set; }
        public int X { get; set; }
        public string Y { get; set; }
    }
}