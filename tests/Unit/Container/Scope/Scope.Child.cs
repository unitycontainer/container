using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container.Scope
{
    [TestClass]
    public partial class ChildScopeTests : ScopeTests
    {
        protected override UnityContainer GetContainer() => (UnityContainer)((IUnityContainer)base.GetContainer()).CreateChildContainer();
    }
}
