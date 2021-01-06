using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Container
{

    [TestClass]
    public partial class Defaults
    {
        #region Constants

        const string INTERFACE     = "Testing";
        const string SET_PATTERN   = "IPolicySet.{0}({1}, {2})";
        const string LIST_PATTERN  = "IPolicyList.{0}({1}, {2}, {3})";
        const string SET_POLICIES  = "IPolicies.{0}({1}, {2})";
        const string LIST_POLICIES = "IPolicies.{0}({1}, {2}, {3})";
        const string EXCHANGE      = nameof(IPolicies);
        const string EXCHANGE_PATTERN = "{1}({2}).CompareExchange({3}, {4})";


        #endregion


        #region Fields

        TestDefaults Policies;
        object Instance = new object();
        private static Type[] TestTypes;
        private int DefaultPolicies = 6;

        Type Target;
        Type Type;
        object Policy;

        #endregion


        #region Scaffolding

        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
            TestTypes = Assembly.GetAssembly(typeof(int))
                                .DefinedTypes
                                .Where(t => t != typeof(IServiceProvider))
                                .Take(1500)
                                .ToArray();
        }


        [TestInitialize]
        public void InitializeTest() => Policies = new TestDefaults();

        #endregion


        #region Implementation

        private void OnPolicyChanged(Type target, Type type, object policy)
        {
            Target = target;
            Type = type;
            Policy = policy;
        }

        #endregion
    }

    #region Test Types

    internal class TestDefaults : Policies<BuilderContext>
    {
        public object SyncObject => SyncRoot;
    }
    
    #endregion
}
