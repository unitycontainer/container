using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Resolution;

namespace Dependency.Overrides
{
    [TestClass]
    public class ParameterOverrideTests : ResolverOverrideTests
    {
        protected override ResolverOverride GetResolverOverride() => new ParameterOverride(string.Empty, OverrideValue);
        protected override ResolverOverride GetNamedResolverOverride() => new ParameterOverride(Name, new TestResolver());
    }
}

