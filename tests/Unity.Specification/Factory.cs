using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Factory
{
    [TestClass]
    public class Resolution : Unity.Specification.Factory.Resolution.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Registration : Unity.Specification.Factory.Registration.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }
}


namespace Resolved.Factory
{
    [TestClass]
    public class Resolution : Unity.Specification.Factory.Resolution.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Registration : Unity.Specification.Factory.Registration.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
