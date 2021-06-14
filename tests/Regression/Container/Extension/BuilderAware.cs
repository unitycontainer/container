using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression.Container;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
using Microsoft.Practices.ObjectBuilder2;
#else
using Unity;
#endif

namespace Container
{
    public partial class Extensions
    {
        [TestMethod, TestProperty(TESTING, nameof(IBuilderAware))]
        public void BuildCallsClassWithInterface()
        {
            var container = new UnityContainer()
                .AddNewExtension<BuilderAwareExtension>();

            var instance = container.Resolve<Aware>();

            Assert.IsTrue(instance.OnBuiltUpWasCalled);
            Assert.IsFalse(instance.OnTearingDownWasCalled);
            Assert.AreEqual(typeof(Aware), instance.Type);
        }

        [TestMethod, TestProperty(TESTING, nameof(IBuilderAware))]
        public void BuildChecksConcreteTypeAndNotRequestedType()
        {
            var container = new UnityContainer()
                .AddNewExtension<BuilderAwareExtension>()
                .RegisterType<Ignorant, Aware>();

            var instance = container.Resolve<Ignorant>();

            Assert.IsTrue(instance.OnBuiltUpWasCalled);
            Assert.IsFalse(instance.OnTearingDownWasCalled);
            Assert.AreEqual(typeof(Aware), instance.Type);
        }

        [TestMethod, TestProperty(TESTING, nameof(IBuilderAware))]
        public void BuildIgnoresClassWithoutInterface()
        {
            var container = new UnityContainer()
                .AddNewExtension<BuilderAwareExtension>()
                .RegisterType<Ignorant, Ignorant>();

            var instance = container.Resolve<Ignorant>();

            Assert.IsFalse(instance.OnBuiltUpWasCalled);
            Assert.IsFalse(instance.OnTearingDownWasCalled);
            Assert.IsNull(instance.Type);
        }
    }


    #region Test Data

    public class Aware : Ignorant, IBuilderAware
    {
    }

    public class Ignorant
    {
        public bool OnBuiltUpWasCalled;
        public bool OnTearingDownWasCalled;
        public Type Type;
        public string Name;

#if UNITY_V4

        public void OnBuiltUp(NamedTypeBuildKey buildKey)
        {
            OnBuiltUpWasCalled = true;
            Type = buildKey.Type;
            Name = buildKey.Name;
        }

        public void OnTearingDown()
        {
            OnTearingDownWasCalled = true;
        }

#else

        public void OnBuiltUp(Type type, string name)
        {
            OnBuiltUpWasCalled = true;
            Type = type;
            Name = name;
        }

#endif
    }

    #endregion
}
