using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Issues
{
    [TestClass]
    public class GitHub : Unity.Specification.Issues.GitHub.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }

    [TestClass]
    public class CodePlex : Unity.Specification.Issues.Codeplex.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
