using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Lifetime.Hierarchies
{
    public abstract partial class Pattern
    {
        #region Registered Singleton

        [TestMethod("Resolving from root to the top child")]
        [DynamicData(nameof(Hierarchy_Middle_Data)), TestProperty(RESOLVING, REGISTRATION_CHILD)]
        public void InMiddleContainer_Singleton(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(SingletonService), typeof(IUnityContainer));

            var child1 = Container.CreateChildContainer()
                                  .RegisterType(typeof(SingletonService), factory());
            var child2 = child1.CreateChildContainer();

            var instance_from_root = Container.Resolve(target) as PatternBaseType;
            var instanceFromChild1 = child1.Resolve(target)    as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target)    as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }


        [TestMethod("Resolving from the top child to root")]
        [DynamicData(nameof(Hierarchy_Middle_Data)), TestProperty(RESOLVING, REGISTRATION_CHILD)]
        public void Child_InParentContainer_Singleton(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {

            var target = type.MakeGenericType(typeof(SingletonService), typeof(IUnityContainer));
            var child1 = Container.CreateChildContainer()
                                  .RegisterType(typeof(SingletonService), factory());
            var child2 = child1.CreateChildContainer();

            var instanceFromChild2 = child2.Resolve(target)    as PatternBaseType;
            var instanceFromChild1 = child1.Resolve(target)    as PatternBaseType;
            var instance_from_root = Container.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> Hierarchy_Middle_Data
        {
            get
            {
                foreach (var member in MemberInfo_Namespace_Names)
                {
                    var definition = GetTestType("BaselineTestType`2", IMPORT_REQUIRED, member);

                    #region HierarchicalLifetimeManager

                    yield return new object[]
                    {
                        typeof(HierarchicalLifetimeManager).Name,
                        definition,                 // Test type
                        (LifetimeManagerFactoryDelegate)(() => new HierarchicalLifetimeManager()),
                        (AssertResolutionDelegate)Import_From_Resolved,
                        (AssertResolutionDelegate)Imports_AreNotSame
                    };

                    #endregion

                    #region ContainerControlledLifetimeManager

                    yield return new object[]
                    {
                        typeof(ContainerControlledLifetimeManager).Name,
                        definition,                 // Test type
                        (LifetimeManagerFactoryDelegate)(() => new ContainerControlledLifetimeManager()),
                        (AssertResolutionDelegate)Import_From_Middle,
                        (AssertResolutionDelegate)Singleton_From_Middle
                    };

                    #endregion
                }
            }
        }

        #endregion
    }
}
