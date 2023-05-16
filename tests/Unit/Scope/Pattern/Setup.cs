using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;
using static Unity.Storage.Scope;

namespace Container.Scope
{
    public abstract partial class ScopeTests
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

        private static readonly IEnumerable<TypeInfo>  _definedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
        protected static string[] TestNames = _definedTypes.Select(t => t.Name)
                                                           .Distinct()
                                                           .Take(100)
                                                           .ToArray();
        protected static Type[] TestTypes = _definedTypes.Where(t => t != typeof(IServiceProvider))
                                                         .Take(2000)
                                                         .ToArray();

        protected static LifetimeManager Manager = new ContainerControlledLifetimeManager
        {
            Data = "Zero",
            Category = RegistrationCategory.Instance
        };

        Contract[] Contracts = Test_Contract_Data.Select(a => new Contract((Type)a[0], (string)a[1]))
                                                 .ToArray();


        protected Unity.Storage.Scope Scope;

        #endregion


        public static IEnumerable<object[]> Test_Contract_Data
        {
            get 
            {
                for (int i = 0; i < 100; i++)
                {
                    yield return new object[] { TestTypes[i], null};
                }

                for (int i = 10; i < 1010; i++)
                {
                    for (int s = 0; s < 100; s++)
                    {
                        yield return new object[] { TestTypes[i], TestNames[s] };
                    }
                }
            }
        }
    }

    public static class ScopeTestExtensions
    {
        public static Entry[] ToArray(this Unity.Storage.Scope sequence)
        {
            return sequence.Memory.Span.ToArray();
        }
    }
}
