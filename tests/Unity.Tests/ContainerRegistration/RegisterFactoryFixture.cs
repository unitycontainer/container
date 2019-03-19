using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Tests.v5.ContainerRegistration
{
    [TestClass]
    public class RegisterFactoryFixture
    {
        [TestMethod]
        public void PassedOverridesMustBeNullIfThereAreNoOverrides()
        {
            const string registrationName = "name";

            var instance = new TypeImplementation();
            var container = new UnityContainer().AddExtension(new Diagnostic());

            container.RegisterFactory(typeof(ITypeInterface), registrationName,
                (c, t, n, o) =>
                {
                    Assert.IsNull(o);
                    return instance;
                },
                new TransientLifetimeManager()
            );

            var result = container.Resolve<ITypeInterface>(registrationName);

            Assert.AreEqual(instance, result);
        }

        [TestMethod]
        public void OverridesMustBePassedToFactory()
        {
            const string registrationName = "name";

            var instance = new TypeImplementation();
            var container = new UnityContainer().AddExtension(new Diagnostic());
            var overrides = new ResolverOverride[] { new FieldOverride("meaningOfLife", 42) };

            container.RegisterFactory(typeof(ITypeInterface), registrationName,
                (c, t, n, o) =>
                {
                    Assert.AreEqual(overrides, o);
                    return instance;
                },
                new TransientLifetimeManager()
            );

            var result = container.Resolve<ITypeInterface>(registrationName, overrides);

            Assert.AreEqual(instance, result);
        }
    }
}