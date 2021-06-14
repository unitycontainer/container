using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Lifetime.Hierarchies
{
    public abstract partial class Pattern : Lifetime.Pattern
    {
        #region Constants

        protected const string REGISTRATION_NONE   = "Unregistered";
        protected const string REGISTRATION_ROOT   = "Registered In Root";
        protected const string REGISTRATION_CHILD  = "Registered In Child";
        protected const string PATTERN_NAME_FORMAT = "{0} {3} in {1} then in {2} ({4})";

        #endregion


        #region Delegates

        public delegate void AssertResolutionDelegate(IUnityContainer root, PatternBaseType fromRoot,
                                                      IUnityContainer child1, PatternBaseType fromChild1,
                                                      IUnityContainer child2, PatternBaseType fromChild2);
        #endregion


        #region Parameters

        public static void TType2_From_Root(IUnityContainer root,   PatternBaseType instance,
                                            IUnityContainer child1, PatternBaseType instance1,
                                            IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreSame(root, instance.Default,  $"{nameof(instance)} should be created in root container");
            Assert.AreSame(root, instance1.Default, $"{nameof(instance1)} should be created in root container");
            Assert.AreSame(root, instance2.Default, $"{nameof(instance2)} should be created in root container");
        }

        public static void TType2_From_Resolved(IUnityContainer root, PatternBaseType instance,
                                                IUnityContainer child1, PatternBaseType instance1,
                                                IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreSame(root,   instance.Default,  $"{nameof(instance)} should be created in root container");
            Assert.AreSame(child1, instance1.Default, $"{nameof(instance1)} should be created in child1 container");
            Assert.AreSame(child2, instance2.Default, $"{nameof(instance2)} should be created in child2 container");
        }

        public static void TTypes2_AreSame(IUnityContainer root, PatternBaseType instance,
                                            IUnityContainer child1, PatternBaseType instance1,
                                            IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreSame(instance.Default, instance1.Default, $"{nameof(instance1)}.Default should be same as {nameof(instance)}.Default");
            Assert.AreSame(instance.Default, instance2.Default, $"{nameof(instance2)}.Default should be same as {nameof(instance)}.Default");
        }

        public static void TTypes2_AreNotSame(IUnityContainer root, PatternBaseType instance,
                                              IUnityContainer child1, PatternBaseType instance1,
                                              IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreNotSame(instance.Default, instance1.Default, $"{nameof(instance1)}.Default should not be the same as {nameof(instance)}.Default");
            Assert.AreNotSame(instance.Default, instance2.Default, $"{nameof(instance2)}.Default should not be the same as {nameof(instance)}.Default");
            Assert.AreNotSame(instance1.Default, instance2.Default, $"{nameof(instance2)}.Default should not be the same as {nameof(instance1)}.Default");
        }


        public static void TTypes1_From_Root(IUnityContainer root, PatternBaseType instance,
                                             IUnityContainer child1, PatternBaseType instance1,
                                             IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreSame(root, instance.Value,  $"{nameof(instance)} should be created in root container");
            Assert.AreSame(root, instance1.Value, $"{nameof(instance1)} should be created in root container");
            Assert.AreSame(root, instance2.Value, $"{nameof(instance2)} should be created in root container");
        }

        public static void TType1_From_Resolved(IUnityContainer root, PatternBaseType instance,
                                                IUnityContainer child1, PatternBaseType instance1,
                                                IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreSame(root, instance.Value, $"{nameof(instance)} should be created in root container");
            Assert.AreSame(child1, instance1.Value, $"{nameof(instance1)} should be created in child1 container");
            Assert.AreSame(child2, instance2.Value, $"{nameof(instance2)} should be created in child2 container");
        }

        public static void TTypes1_AreSame(IUnityContainer root, PatternBaseType instance,
                                           IUnityContainer child1, PatternBaseType instance1,
                                           IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreSame(instance.Value, instance1.Value, $"{nameof(instance1)}.Value should be same as {nameof(instance)}.Value");
            Assert.AreSame(instance.Value, instance2.Value, $"{nameof(instance2)}.Value should be same as {nameof(instance)}.Value");
        }

        public static void TTypes1_AreNotSame(IUnityContainer root, PatternBaseType instance,
                                              IUnityContainer child1, PatternBaseType instance1,
                                              IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreNotSame(instance.Value, instance1.Value, $"{nameof(instance1)}.Value should not be the same as {nameof(instance)}.Value");
            Assert.AreNotSame(instance.Value, instance2.Value, $"{nameof(instance2)}.Value should not be the same as {nameof(instance)}.Value");
            Assert.AreNotSame(instance1.Value, instance2.Value, $"{nameof(instance2)}.Value should not be the same as {nameof(instance1)}.Value");
        }

        public static void Items_AreNotSame(IUnityContainer root, PatternBaseType instance,
                                            IUnityContainer child1, PatternBaseType instance1,
                                            IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreNotSame(instance, instance1, $"{nameof(instance1)} should not be the same as {nameof(instance)}");
            Assert.AreNotSame(instance, instance2, $"{nameof(instance2)} should not be the same as {nameof(instance)}");
            Assert.AreNotSame(instance1, instance2, $"{nameof(instance2)} should not be the same as {nameof(instance1)}");
        }

        #endregion


        #region Import

        public static void Import_From_Root(IUnityContainer root, PatternBaseType instance,
                                             IUnityContainer child1, PatternBaseType instance1,
                                             IUnityContainer child2, PatternBaseType instance2)
        {
            var host  = (instance.Value  as PatternBaseType).Default;
            var host1 = (instance1.Value as PatternBaseType).Default;
            var host2 = (instance2.Value as PatternBaseType).Default;

            Assert.AreSame(root, host,  $"{nameof(instance)} should be created in root container");
            Assert.AreSame(root, host1, $"{nameof(instance1)} should be created in root container");
            Assert.AreSame(root, host2, $"{nameof(instance2)} should be created in root container");
        }
         
        public static void Import_From_Resolved(IUnityContainer root, PatternBaseType instance,
                                                   IUnityContainer child1, PatternBaseType instance1,
                                                   IUnityContainer child2, PatternBaseType instance2)
        {
            var host = (instance.Value as PatternBaseType).Default;
            var host1 = (instance1.Value as PatternBaseType).Default;
            var host2 = (instance2.Value as PatternBaseType).Default;

            Assert.AreSame(root,   host,  $"{nameof(instance)} should be created in root container");
            Assert.AreSame(child1, host1, $"{nameof(instance1)} should be created in child1 container");
            Assert.AreSame(child2, host2, $"{nameof(instance2)} should be created in child2 container");
        }

        public static void Imports_AreSame(IUnityContainer root, PatternBaseType instance,
                                             IUnityContainer child1, PatternBaseType instance1,
                                             IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreSame(instance.Value, instance1.Value, $"{nameof(instance1)}.Value should be same as {nameof(instance)}.Value");
            Assert.AreSame(instance.Value, instance2.Value, $"{nameof(instance2)}.Value should be same as {nameof(instance)}.Value");
        }

        public static void Imports_AreNotSame(IUnityContainer root, PatternBaseType instance,
                                                IUnityContainer child1, PatternBaseType instance1,
                                                IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreNotSame(instance.Value, instance1.Value, $"{nameof(instance1)}.Value should not be the same as {nameof(instance)}.Value");
            Assert.AreNotSame(instance.Value, instance2.Value, $"{nameof(instance2)}.Value should not be the same as {nameof(instance)}.Value");
            Assert.AreNotSame(instance1.Value, instance2.Value, $"{nameof(instance2)}.Value should not be the same as {nameof(instance1)}.Value");
        }

        #endregion


        #region Middle

        public static void Import_From_Middle(IUnityContainer root, PatternBaseType instance,
                                              IUnityContainer child1, PatternBaseType instance1,
                                              IUnityContainer child2, PatternBaseType instance2)
        {
            var host = (instance.Value as PatternBaseType).Default;
            var host1 = (instance1.Value as PatternBaseType).Default;
            var host2 = (instance2.Value as PatternBaseType).Default;

            Assert.AreSame(root, host, $"{nameof(instance)} should be created in root container");
            Assert.AreSame(host1, host2, $"{nameof(instance2)} should not be created in child1 container");

            Assert.AreNotSame(root, host1, $"{nameof(instance1)} should not be created in root container");
            Assert.AreNotSame(root, host2, $"{nameof(instance2)} should not be created in root container");
        }

        public static void Singleton_From_Middle(IUnityContainer root, PatternBaseType instance,
                                                 IUnityContainer child1, PatternBaseType instance1,
                                                 IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreNotSame(instance.Value, instance1.Value, $"{nameof(instance1)}.Value should not be the same as {nameof(instance)}.Value");
            Assert.AreNotSame(instance.Value, instance2.Value, $"{nameof(instance2)}.Value should not be the same as {nameof(instance)}.Value");
            Assert.AreSame(instance1.Value, instance2.Value, $"{nameof(instance2)}.Value should be same as {nameof(instance1)}.Value");
        }

        public static void Middle_AreNotSame(IUnityContainer root, PatternBaseType instance,
                                                IUnityContainer child1, PatternBaseType instance1,
                                                IUnityContainer child2, PatternBaseType instance2)
        {
            Assert.AreNotSame(instance1.Value, instance2.Value, $"{nameof(instance2)}.Value should not be the same as {nameof(instance1)}.Value");
        }

        #endregion


        #region Test Data

        public class SingletonService : PatternBaseType, IDisposable
        {
            public SingletonService(IUnityContainer container)
            {
                Default = container;
                Value = container.GetHashCode();
            }

            public override object Default
            {
                get { return (base.Default as WeakReference)?.Target; }
                protected set => base.Default = new WeakReference(value);
            }

            public bool IsDisposed { get; private set; }
            public void Dispose() => IsDisposed = true;
        }

        #endregion

    }
}
