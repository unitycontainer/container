using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Resolution
{
    [TestClass]
    public partial class Enumerables : PatternBase
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => PatternBaseInitialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Test Data

        public interface IConstrained<TEntity>
            where TEntity : IService
        {
            TEntity Value { get; }
        }

        public class Constrained<TEntity> : IConstrained<TEntity>
            where TEntity : Service
        {
            public Constrained()
            {
            }

            public Constrained(TEntity value)
            {
                Value = value;
            }

            public TEntity Value { get; }
        }


        #endregion
    }
}
