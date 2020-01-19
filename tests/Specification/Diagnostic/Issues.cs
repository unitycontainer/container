using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Diagnostic.Issues.GitHub;

namespace GitHub
{
    [TestClass]
    public class Container : Unity.Specification.Diagnostic.Issues.GitHub.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic);
        }
    }

}
