using System;

namespace Unity.Tests.CollectionSupport
{
    public class TestClass : ITestInterface
    {
        public string ID { get; } = Guid.NewGuid().ToString();
    }
}