using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.BuiltIn;
using Unity.Lifetime;

namespace Container.Scope
{
    [TestClass]
    public partial class EnumeratorTests
    {
        protected string Name = "name";
        protected static LifetimeManager Manager = new ContainerControlledLifetimeManager
        {
            Data = "Zero",
            Category = RegistrationCategory.Instance
        };

        protected Unity.Container.Scope Scope;

        [TestInitialize]
        public virtual void InitializeTest() => Scope = new ContainerScope(5);
    }
}
