using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif

namespace Resolution
{
    [TestClass]
    public partial class Deferred : PatternBase
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            Container.RegisterType(typeof(IList<>), typeof(List<>), new InjectionConstructor());
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            Container.RegisterType<IService, Service>("1");
            Container.RegisterType<IService, Service>("2");
            Container.RegisterType<IService, OtherService>("3");
            Container.RegisterType<IService, Service>();

            Service.Instances = 0;
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context) => PatternBaseInitialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Implementation

        public static void AreEquivalent(ICollection expected, ICollection actual)
        {
            if (Equals(expected, actual))
            {
                return;
            }

            if (expected.Count != actual.Count)
            {
                throw new AssertFailedException("collections differ in size");
            }

            var expectedCounts = expected.Cast<object>().GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count());
            var actualCounts = actual.Cast<object>().GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count());

            foreach (var kvp in expectedCounts)
            {
                if (actualCounts.TryGetValue(kvp.Key, out var actualCount))
                {
                    if (actualCount != kvp.Value)
                    {
                        throw new AssertFailedException(string.Format(CultureInfo.InvariantCulture, "collections have different count for element {0}", kvp.Key));
                    }
                }
                else
                {
                    throw new AssertFailedException(string.Format(CultureInfo.InvariantCulture, "actual does not contain element {0}", kvp.Key));
                }
            }
        }

        #endregion


        #region Test Data

        public class ObjectThatGetsAResolver
        {
            [Dependency]
            public Func<IService> LoggerResolver { get; set; }
        }


        #endregion
    }
}
