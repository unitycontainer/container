using System;

namespace Unity.Tests.TestDoubles
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DependencyAttribute : Attribute
    {
    }
}
