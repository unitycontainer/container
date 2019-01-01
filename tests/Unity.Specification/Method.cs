using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Method
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }
}

namespace Resolved.Method
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }
}
