using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Parameter
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injected.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }
}

namespace Resolved.Parameter
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injected.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }
}
