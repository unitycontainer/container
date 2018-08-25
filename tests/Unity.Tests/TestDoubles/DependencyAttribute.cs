using System;

namespace Unity.Tests.v5.TestDoubles
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DependencyAttribute : Attribute
    {
    }
}
