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
    public partial class Builder
    {
        #region Constants

        const string TEST = "Testing";
        const string RESOLVE  = "Resolution";
        const string EXPRESS  = "Expression";
        const string ACTIVATE = "Activation";
        const string BUILDUP = "BuildUp"; 

        #endregion


        #region Fields

        StagedStrategyChain Chain;
        FakeContext Context;


        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize()
        {
            Chain = new StagedStrategyChain();
            Context = new FakeContext();
        }

        #endregion


        #region Test Data





        #endregion


        #region Fake Context

        private struct FakeContext : IBuilderContext
        {
            private object _data;

            public bool IsFaulted { get; set; }
            public object Target { get => _data; set => _data = value; }
            public object Existing { get => _data; set => _data = value; }


            public IPolicies Policies => throw new NotImplementedException();

            public ResolverOverride[] Overrides => throw new NotImplementedException();
            public RegistrationManager Registration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public object CurrentOperation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public UnityContainer Container => throw new NotImplementedException();
            public Type Type => throw new NotImplementedException();
            public string Name => throw new NotImplementedException();

            public ref Contract Contract => throw new NotImplementedException();

            public ref ErrorInfo ErrorInfo => throw new NotImplementedException();

            public object Capture(Exception exception) => throw new NotImplementedException();

            public void Clear(Type type, Type policy) => throw new NotImplementedException();

            public void Clear(Type type) => throw new NotImplementedException();

            public PipelineContext CreateContext(ref Contract contract, ref ErrorInfo error) => throw new NotImplementedException();

            public PipelineContext CreateContext(ref Contract contract) => throw new NotImplementedException();

            public PipelineContext CreateMap(ref Contract contract) => throw new NotImplementedException();

            public object Error(string error)
            {
                IsFaulted = true;
                return UnityContainer.NoValue;
            }

            public object Get(Type type, Type policy) => throw new NotImplementedException();

            public object Get(Type type) => throw new NotImplementedException();

            public object Resolve(Type type, string name) => throw new NotImplementedException();

            public void Set(Type type, Type policy, object instance) => throw new NotImplementedException();

            public void Set(Type type, object policy) => throw new NotImplementedException();

            public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class => throw new NotImplementedException();
        }

        #endregion
    }

}
