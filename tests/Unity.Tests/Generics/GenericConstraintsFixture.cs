using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Exceptions;

namespace Unity.Tests.v5.Generics
{
    [TestClass]
    public class GenericConstraintsFixture
    {
        interface IFoo<T>
        {
        }

        class Foo<T> : IFoo<T> where T : struct
        {
        }

        [TestMethod]
        public void ThrowsAppropriateExceptionWhenGenericArgumentFailsToMeetConstraintsOfMappedToType()
        {
            var ioc = new UnityContainer();

            ioc.RegisterType(typeof(IFoo<>), typeof(Foo<>));

            Assert.ThrowsException<ResolutionFailedException>(() => ioc.Resolve<IFoo<string>>());
        }

        [TestMethod]
        public void CanResolveOpenGenericInterfaceWithConstraintsInMappedToTypeWhenConstraintsAreMet()
        {
            var ioc = new UnityContainer();

            ioc.RegisterType(typeof(IFoo<>), typeof(Foo<>));

            Assert.IsNotNull(ioc.Resolve<IFoo<int>>());
        }
    }
}
