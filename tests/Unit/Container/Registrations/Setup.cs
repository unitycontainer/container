using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;

namespace Container
{
    [TestClass]
    public partial class Registrations
    {
        private string Name = "name";
        protected static IInstanceLifetimeManager Manager = new ContainerControlledLifetimeManager();
        private static IEnumerable<TypeInfo> DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
        protected static string[] TestNames = Enumerable.Repeat<string>(null, 4)
                                                        .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                                                        .Concat(DefinedTypes.Select(t => t.Name).Distinct().Take(100))
                                                        .ToArray();
        protected static Type[] TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                                        .ToArray();
        protected static object Instance = new object();

        private UnityContainer Container;


        [TestInitialize]
        public virtual void InitializeTest()
        {
            Container = new UnityContainer();
        }


    }
}
