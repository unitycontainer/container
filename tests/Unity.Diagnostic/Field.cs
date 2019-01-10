using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;

namespace Compiled.Field
{

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Method.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }
}

namespace Resolved.Field
{

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Method.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }
}
