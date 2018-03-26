using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Lifetime;

namespace Unity.Tests.Generics
{
    [TestClass]
    public class OpenGenericFixture
    {

        public interface IMygenericTest<T>
        {

        }

        public class MyGenericTest<T> : IMygenericTest<T>
        {

        }

        [TestMethod]
        public void OpenGenericTypesShouldNotBeSame()
        {


            var c = new UnityContainer();

            var c1 = c.CreateChildContainer();
            var c2 = c.CreateChildContainer();

            c1.RegisterType(typeof(IMygenericTest<>), typeof(MyGenericTest<>), new ContainerControlledLifetimeManager());



            var t1 = c1.Resolve<IMygenericTest<int>>();
            Assert.IsNotNull(t1);

               c2.RegisterType(typeof(IMygenericTest<>), typeof(MyGenericTest<>), new ContainerControlledLifetimeManager());


            var t2 = c2.Resolve<IMygenericTest<int>>();
            Assert.IsNotNull(t2);

            Assert.AreNotSame(t2, t1);

        }
    }
}
