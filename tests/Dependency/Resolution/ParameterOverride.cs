using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Resolution;

namespace Dependency.Overrides
{
    [TestClass]
    public class ParameterOverride : ResolverOverrideTests
    {
        protected override ResolverOverride GetResolverOverride() => new Unity.Resolution.ParameterOverride(string.Empty, OverrideValue);
        protected override ResolverOverride GetNamedResolverOverride() => new Unity.Resolution.ParameterOverride(Name, new TestResolver());
    }
}

