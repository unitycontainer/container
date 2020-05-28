using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Resolution;

namespace Dependency.Overrides
{
    [TestClass]
    public class DependencyOverrideTests : ResolverOverrideTests
    {
        protected override ResolverOverride GetResolverOverride() => new DependencyOverride(typeof(object), OverrideValue);
        protected override ResolverOverride GetNamedResolverOverride() => new DependencyOverride(nameof(Name), new TestResolver());

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public virtual void GetMockResolver()
        {
            var instance = new MockResolverOverride();

            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
        }

        public class MockResolverOverride : ResolverOverride
        {
            public MockResolverOverride()
                : base(ResolverOverrideTests.Name)
            {

            }
        }
    }
}
