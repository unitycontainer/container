using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Issues
{
    [TestClass]
    public class GitHub : Unity.Specification.Diagnostic.Issues.GitHub.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddNewExtension<Diagnostic>();
        }
    }

    [TestClass]
    public class CodePlex : Unity.Specification.Issues.Codeplex.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddNewExtension<Diagnostic>();
        }
    }
}
