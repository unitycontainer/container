using System;

namespace Unit.Test.TestDoubles
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DependencyAttribute : Attribute
    {
    }
}
