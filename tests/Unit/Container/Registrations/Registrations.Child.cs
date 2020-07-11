using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container.Registrations
{
    [TestClass]
    public class RegistrationsInChildTests : RegistrationsTests
    {
        [TestInitialize]
        public override void InitializeTest() => Container = 
            (UnityContainer)((IUnityContainer)new UnityContainer()).CreateChildContainer();
    }
}
