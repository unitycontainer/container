using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Container
{
    public partial class Pattern
    {
        [TestMethod("A dependency on the container is injected")]
        [DynamicData(nameof(Hierarchy_Test_Data)), TestProperty(IMPORTING, nameof(IUnityContainer))]
        public void IUnityContainer_Injected(string name, Type definition, bool isNamed)
        {
            if (isNamed) return;

            var type = definition.MakeGenericType(typeof(IUnityContainer));
            var instance = Container.Resolve(type) as PatternBaseType;
            Assert.AreSame(Container, instance.Value);
        }

        [TestMethod("A dependency on the child container is injected")]
        [DynamicData(nameof(Hierarchy_Test_Data)), TestProperty(IMPORTING, nameof(IUnityContainer))]
        public void IUnityContainer_Child(string name, Type definition, bool isNamed)
        {
            if (isNamed) return;

            var type = definition.MakeGenericType(typeof(IUnityContainer));
            IUnityContainer childContainer = Container.CreateChildContainer();
            var instance = childContainer.Resolve(type) as PatternBaseType;
            Assert.AreSame(childContainer, instance.Value);
        }

#if !UNITY_V4 && !UNITY_V5 && !UNITY_V6

        [TestMethod("A dependency on the container is injected")]
        [DynamicData(nameof(Hierarchy_Test_Data)), TestProperty(IMPORTING, nameof(IServiceProvider))]
        public void IServiceProvider_Injected(string name, Type definition, bool isNamed)
        {
            if (isNamed) return;

            var type = definition.MakeGenericType(typeof(IServiceProvider));
            var instance = Container.Resolve(type) as PatternBaseType;
            Assert.AreSame(Container, instance.Value);
        }

        [TestMethod("A dependency on the child container is injected")]
        [DynamicData(nameof(Hierarchy_Test_Data)), TestProperty(IMPORTING, nameof(IServiceProvider))]
        public void IServiceProvider_Child(string name, Type definition, bool isNamed)
        {
            if (isNamed) return;

            var type = definition.MakeGenericType(typeof(IServiceProvider));
            IUnityContainer childContainer = Container.CreateChildContainer();
            var instance = childContainer.Resolve(type) as PatternBaseType;
            Assert.AreSame(childContainer, instance.Value);
        }
#endif
    }
}
