using BenchmarkDotNet.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Benchmark.Tests
{
    [TestClass]
    public class BenchmarkTestBase<TBenchmark>
        where TBenchmark : class, new()
    {
        #region Fields

        private   static Action     IterationSetupAction;
        private   static Action     IterationCleanupAction;
        protected static TBenchmark Benchmark = new TBenchmark();

        #endregion


        #region Constructors

        static BenchmarkTestBase()
        {
            var methods = typeof(TBenchmark).GetMethods();
            var globalSetup = methods.Where(info => info.IsDefined(typeof(GlobalSetupAttribute), true))
                                     .FirstOrDefault();
            globalSetup?.Invoke(null, Array.Empty<object>());

            var method = methods.Where(info => info.IsDefined(typeof(IterationSetupAttribute), true))
                                .FirstOrDefault();
            
            if (method is not null)
                IterationSetupAction = (Action)method.CreateDelegate(typeof(Action), Benchmark);

            method = methods.Where(info => info.IsDefined(typeof(IterationCleanupAttribute), true))
                            .FirstOrDefault();

            if (method is not null)
                IterationCleanupAction = (Action)method.CreateDelegate(typeof(Action), Benchmark);
        }

        #endregion


        #region Scaffolding

        [TestInitialize]
        public virtual void IterationSetup() => IterationSetupAction?.Invoke();

        [TestCleanup]
        public virtual void IterationCleanup() => IterationCleanupAction?.Invoke();

        #endregion
    }
}
