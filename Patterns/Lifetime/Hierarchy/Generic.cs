using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
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
        #region Generic Direct

#if !BEHAVIOR_V4
        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Dependency_Data))]
        public void Generic_Root_Children_Type_Siblings(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(IUnityContainer), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = Container.CreateChildContainer();

            var instance_from_root = Container.Resolve(target) as PatternBaseType;
            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            { 
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }


        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Dependency_Data))]
        public void Generic_Root_Children_Type_Hierarchical(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(IUnityContainer), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            var instance_from_root = Container.Resolve(target) as PatternBaseType;
            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }


        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Dependency_Data))]
        public void Generic_Children_Root_Type_Siblings(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(IUnityContainer), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = Container.CreateChildContainer();

            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instance_from_root = Container.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }


        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Dependency_Data))]
        public void Generic_Children_Root_Type_Hierarchical(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(IUnityContainer), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instance_from_root = Container.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }
#endif

        #endregion


        #region Unregistered Import

#if !BEHAVIOR_V4
        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Import_Data))]
        public void Generic_Root_Children_Import_Siblings(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(SingletonService), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = Container.CreateChildContainer();

            var instance_from_root = Container.Resolve(target) as PatternBaseType;
            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }

        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Import_Data))]
        public void Generic_Root_Children_Import_Hierarchical(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(SingletonService), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            var instance_from_root = Container.Resolve(target) as PatternBaseType;
            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }


        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Import_Data))]
        public void Generic_Children_Root_Import_Siblings(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(SingletonService), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = Container.CreateChildContainer();

            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;
            var instance_from_root = Container.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }

        [PatternTestMethod(PATTERN_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Hierarchy_Import_Data))]
        public void Generic_Children_Root_Import_Hierarchical(string name, Type type, LifetimeManagerFactoryDelegate factory, params AssertResolutionDelegate[] methods)
        {
            var target = type.MakeGenericType(typeof(SingletonService), typeof(IUnityContainer));
            Container.RegisterType(type, factory());

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            var instanceFromChild1 = child1.Resolve(target) as PatternBaseType;
            var instanceFromChild2 = child2.Resolve(target) as PatternBaseType;
            var instance_from_root = Container.Resolve(target) as PatternBaseType;

            foreach (var assert in methods)
            {
                assert(Container, instance_from_root,
                          child1, instanceFromChild1,
                          child2, instanceFromChild2);
            }
        }
#endif

        #endregion
    }
}
