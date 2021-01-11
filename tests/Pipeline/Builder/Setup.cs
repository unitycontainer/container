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
            public object Existing { get => _data; set => _data = value; }


            public IPolicies Policies => throw new NotImplementedException();

            public ResolverOverride[] Overrides => throw new NotImplementedException();
            public RegistrationManager Registration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public object CurrentOperation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public UnityContainer Container => throw new NotImplementedException();
            public Type Type => throw new NotImplementedException();
            public string Name => throw new NotImplementedException();
            public ref Contract Contract => throw new NotImplementedException();
            ref ErrorDescriptor IBuilderContext.ErrorInfo => throw new NotImplementedException();
            public object PerResolve { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public object Capture(Exception exception) => throw new NotImplementedException();

            public object Error(string error)
            {
                IsFaulted = true;
                return UnityContainer.NoValue;
            }

            public object Resolve(Type type, string name) => throw new NotImplementedException();
            public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class => throw new NotImplementedException();
            public object MapTo(Contract contract) => throw new NotImplementedException();
            public object FromContract(Contract contract) => throw new NotImplementedException();
            public object FromContract(Contract contract, ref ErrorDescriptor errorInfo) => throw new NotImplementedException();
            public object FromPipeline(Contract contract, Delegate pipeline) => throw new NotImplementedException();
            public ResolverOverride GetOverride<TMemberInfo, TDescriptor>(ref TDescriptor descriptor) where TDescriptor : IImportDescriptor<TMemberInfo> => throw new NotImplementedException();
        }

        #endregion
    }

}
