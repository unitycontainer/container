using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;

namespace Pipeline
{
    [TestClass]
    public partial class BuilderTests
    {
        #region Constants

        const string TEST = "Testing";
        const string ACTIVATE = "Activation";

        #endregion


        #region Fields

        StagedChain<UnityBuildStage, BuilderStrategy> Chain;
        FakeContext Context;


        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize()
        {
            Chain = new StagedChain<UnityBuildStage, BuilderStrategy>();
            Context = new FakeContext();
        }

        #endregion


        #region Test Data

        public class NoStrategy : BuilderStrategy
        { 
        }

        public class PreBuildUpStrategy : BuilderStrategy
        {
            public override void PreBuildUp<TContext>(ref TContext context)
            {
            }
        }

        public class PostBuildUpStrategy : BuilderStrategy
        {
            public override void PostBuildUp<TContext>(ref TContext context)
            {
            }
        }

        public class BothStrategies : BuilderStrategy
        {
            public override void PreBuildUp<TContext>(ref TContext context)
            {
            }

            public override void PostBuildUp<TContext>(ref TContext context)
            {
            }
        }

        public class FaultedStrategy : BuilderStrategy
        {
            public override void PreBuildUp<TContext>(ref TContext context)
            {
                context.Error("Error");
            }

            public override void PostBuildUp<TContext>(ref TContext context)
            {
                Assert.Fail();
            }
        }

        #endregion


        #region Fake Context

        private struct FakeContext : IBuilderContext
        {
            public bool IsFaulted { get; set; }


            public IPolicies Policies => throw new NotImplementedException();

            public object Target { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public object Existing { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public ResolverOverride[] Overrides => throw new NotImplementedException();
            public RegistrationManager Registration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public object CurrentOperation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public UnityContainer Container => throw new NotImplementedException();
            public Type Type => throw new NotImplementedException();
            public string Name => throw new NotImplementedException();

            public object Capture(Exception exception)
            {
                throw new NotImplementedException();
            }

            public void Clear(Type type, Type policy)
            {
                throw new NotImplementedException();
            }

            public void Clear(Type type)
            {
                throw new NotImplementedException();
            }

            public PipelineContext CreateContext(ref Contract contract, ref ErrorInfo error)
            {
                throw new NotImplementedException();
            }

            public PipelineContext CreateContext(ref Contract contract)
            {
                throw new NotImplementedException();
            }

            public object Error(string error)
            {
                IsFaulted = true;
                return UnityContainer.NoValue;
            }

            public object Get(Type type, Type policy)
            {
                throw new NotImplementedException();
            }

            public object Get(Type type)
            {
                throw new NotImplementedException();
            }

            public object Resolve(Type type, string name)
            {
                throw new NotImplementedException();
            }

            public void Set(Type type, Type policy, object instance)
            {
                throw new NotImplementedException();
            }

            public void Set(Type type, object policy)
            {
                throw new NotImplementedException();
            }

            public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }

}
