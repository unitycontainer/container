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
        [TestMethod("Resolve in child using parent registration")]
        [DynamicData(nameof(Hierarchy_Test_Data)), TestCategory(HIERARCHY)]
        public void Register_Root_Resolve_Child(string name, Type definition, bool isNamed)
        {
            Container.RegisterType<IService, Service>()
                     .RegisterType<IService, OtherService>(Name);

            var target = definition.MakeGenericType(typeof(IService));
            
            var instance = Container.CreateChildContainer()
                                    .Resolve(target) as PatternBaseType;

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);
            Assert.IsInstanceOfType(instance.Value, isNamed ? typeof(OtherService) : typeof(Service));
        }


        [TestMethod("Child registration overrides parent")]
        [DynamicData(nameof(Hierarchy_Test_Data)), TestCategory(HIERARCHY)]
        public void ChildOverridesParent(string name, Type definition, bool isNamed)
        {
            var target = definition.MakeGenericType(typeof(IService));
            
            Container.RegisterType<IService, Service>()
                     .RegisterType<IService, OtherService>(Name);


            var instance = Container.CreateChildContainer()
                                    .RegisterType<IService, OtherService>()
                                    .RegisterType<IService, Service>(Name)
                                    .Resolve(target) as PatternBaseType;

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);
            Assert.IsInstanceOfType(instance.Value, isNamed ? typeof(Service) : typeof(OtherService));
        }


        [TestMethod("Changes in parent reflects in child")]
        [DynamicData(nameof(Hierarchy_Test_Data)), TestCategory(HIERARCHY)]
        public void ChangesInParent(string name, Type definition, bool isNamed)
        {
            var target = definition.MakeGenericType(typeof(IService));

            Container.RegisterType<IService, Service>()
                     .RegisterType<IService, OtherService>(Name);
            
            var child = Container.CreateChildContainer();

            var instance = child.Resolve(target) as PatternBaseType;

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);
            Assert.IsInstanceOfType(instance.Value, isNamed ? typeof(OtherService) : typeof(Service));

            Container.RegisterType<IService, OtherService>()
                     .RegisterType<IService, Service>(Name);
            
            instance = child.Resolve(target) as PatternBaseType;

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);
            Assert.IsInstanceOfType(instance.Value, isNamed ? typeof(Service) : typeof(OtherService));
        }
    }
}
