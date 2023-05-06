using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;
using static Unity.Storage.Scope;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        #region Constants

        const string Name = "name";
        const string TESTING_IUC  = "IUnityContainer";
        const string TESTING_SPAN = "Descriptor";
        const string TRAIT_ADD = "Add";
        const string TRAIT_GET = "Get";
        const string TRAIT_CONTAINS = "Contains";

        #endregion


        #region Fields

        protected static Type[] TestTypes;
        protected static string[] TestNames;
        protected static LifetimeManager Manager = new ContainerControlledLifetimeManager
        {
            Data = "Zero",
            Category = RegistrationCategory.Instance
        };

        protected Unity.Storage.Scope Scope;

        #endregion


        #region Scaffolding

        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
            var DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
            TestNames = Enumerable.Repeat<string>(null, 4)
                .Concat(DefinedTypes.Select(t => t.Name).Take(5))
                .Concat(DefinedTypes.Select(t => t.Name).Distinct().Take(100))
                .ToArray();
            TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                    .Take(2000)
                                    .ToArray();
        }

        #endregion
    }

    public static class ScopeTestExtensions
    {
        public static Entry[] ToArray(this Unity.Storage.Scope sequence)
        {
            return sequence.Memory.Span.ToArray();
        }
    }
}
