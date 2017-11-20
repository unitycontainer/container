using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Specification.Issues;

namespace Unity.Specification
{
    [TestClass]
    public class ReportedIssuesTestsFixture : ReportedIssuesTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
