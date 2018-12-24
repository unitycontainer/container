using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Resolution.Field;

namespace Resolution
{
    [TestClass]
    public class Field : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
