using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity;
using Unity.Container;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;

namespace Pipeline
{
    [TestClass]
    public partial class StagedChain
    {
        #region Constants

        const string TEST              = "Testing";
        const string ADD               = nameof(IDictionary.Add);
        const string INDEXER           = "Indexer";
        const string CONTAINS          = nameof(IDictionary.Contains);
        const string REMOVE            = nameof(IDictionary.Remove);
        const string TO_ARRAY          = "ToArray()";
        const string BUILDUP           = "BuildUp";
        const string BUILDUP_COMPILED  = "BuildUp Compiled";
        const string ANALYSIS          = "Analysis";
        const string ANALYSIS_COMPILED = "Analysis Compiled";


        #endregion


        #region Fields

        StagedStrategyChain Chain;
        FakeContext Context;

        static readonly Unresolvable Segment0 = Unresolvable.Create("0");
        static readonly Unresolvable Segment1 = Unresolvable.Create("1");
        static readonly Unresolvable Segment2 = Unresolvable.Create("2");
        static readonly Unresolvable Segment3 = Unresolvable.Create("3");
        static readonly Unresolvable Segment4 = Unresolvable.Create("4");
        static readonly Unresolvable Segment5 = Unresolvable.Create("5");

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize()
        {
            Chain = new StagedStrategyChain();
            Context = new FakeContext()
            { 
                Existing = new List<string>()
            };
        }

        #endregion


        #region Test Data

        public class Unresolvable : BuilderStrategy
        {
            public readonly string Id;

            protected Unresolvable(string id) { Id = id; }

            public static Unresolvable Create(string name) => new Unresolvable(name);

            public override string ToString() => $"Unresolvable.{Id}";
        }

        #endregion


        #region Fake Context

        private struct FakeContext : IBuilderContext
        {
            private object _data;

            public int Count => (_data as IList<string>)?.Count ?? 0;

            public object Existing { get => _data; set => _data = value; }

            public bool IsFaulted { get; set; }

            public object Error(string error)
            {
                IsFaulted = true;
                return UnityContainer.NoValue;
            }

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
            public Type Generic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public object Capture(Exception exception) => throw new NotImplementedException();
            public object Resolve(Type type, string name) => throw new NotImplementedException();
            public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class => throw new NotImplementedException();
            public object MapTo(Contract contract) => throw new NotImplementedException();
            public object FromContract(Contract contract) => throw new NotImplementedException();
            public object FromContract(Contract contract, ref ErrorDescriptor errorInfo) => throw new NotImplementedException();
            public object FromPipeline(Contract contract, Delegate pipeline) => throw new NotImplementedException();

            public InjectionMember<TMemberInfo, TData> OfType<TMemberInfo, TData>()
                where TMemberInfo : MemberInfo
                where TData : class => throw new NotImplementedException();
            public ResolverOverride GetOverride<TMemberInfo, TDescriptor>(ref TDescriptor descriptor) where TDescriptor : IImportMemberDescriptor<TMemberInfo> => throw new NotImplementedException();
            public IEnumerable<T> OfType<T>() => throw new NotImplementedException();
            public object Get(Type type) => throw new NotImplementedException();
            public void Set(Type type, object policy) => throw new NotImplementedException();
            public void Clear(Type type) => throw new NotImplementedException();
        }

        #endregion
    }
}
