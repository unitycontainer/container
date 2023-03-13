using System;

namespace Unit.Test.CollectionSupport
{
    public class TestClass : ITestInterface
    {
        public string ID { get; } = Guid.NewGuid().ToString();
    }
}