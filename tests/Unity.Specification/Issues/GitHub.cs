using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Issues;

namespace Issues
{
    [TestClass]
    public class GitHub : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
