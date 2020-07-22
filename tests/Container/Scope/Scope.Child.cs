using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container.Scope
{
    [TestClass]
    public class ChildScope : ScopeTests
    {
        protected override UnityContainer GetContainer()
        {
            return (UnityContainer)((IUnityContainer)base.GetContainer()).CreateChildContainer();
        }
    }
}
