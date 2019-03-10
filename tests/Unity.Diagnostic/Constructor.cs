using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;

namespace Compiled.Constructor
{
    [TestClass]
    public class Annotation : Unity.Specification.Diagnostic.Constructor.Annotation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Diagnostic.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Constructor.Types.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Constructor.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }
}

namespace Resolved.Constructor
{
    [TestClass]
    public class Annotation : Unity.Specification.Diagnostic.Constructor.Annotation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Diagnostic.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Constructor.Types.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Constructor.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }
}
