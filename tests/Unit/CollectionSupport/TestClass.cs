using System;

namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClass : ITestInterface
    {
        public string ID { get; } = Guid.NewGuid().ToString();
    }
}