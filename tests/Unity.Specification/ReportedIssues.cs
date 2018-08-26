using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Issues;

namespace Specification.Tests
{
    [TestClass]
    public class ReportedIssues : ReportedIssuesTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
