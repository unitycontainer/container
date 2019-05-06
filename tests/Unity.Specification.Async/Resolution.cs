using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{
    [TestClass]
    public class AsyncBasics : Unity.Specification.Resolution.Basics.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }
}


namespace Resolved
{
    [TestClass]
    public class AsyncBasics : Unity.Specification.Resolution.Basics.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }
}
