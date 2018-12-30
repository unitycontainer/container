using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Issues.Codeplex;

namespace Issues
{
    [TestClass]
    public class CodePlex : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
