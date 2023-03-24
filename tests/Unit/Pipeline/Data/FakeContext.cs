﻿using System;
using System.Collections.Generic;
using Unity;
using Unity.Container;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;

namespace Pipeline
{
    public struct FakeContext : IBuilderContext
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
        public ref Contract Contract => ref Contract;
        ref ErrorDescriptor IBuilderContext.ErrorInfo => throw new NotImplementedException();
        public object PerResolve { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Type TypeDefinition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ResolverOverride[] IBuilderContext.Overrides => throw new NotImplementedException();

        public object Capture(Exception exception) => throw new NotImplementedException();
        public object Resolve(Type type, string name) => throw new NotImplementedException();
        public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class => throw new NotImplementedException();
        public object MapTo(Contract contract) => throw new NotImplementedException();
        public object FromContract(Contract contract) => throw new NotImplementedException();
        public object FromContract(Contract contract, ref ErrorDescriptor errorInfo) => throw new NotImplementedException();
        public object FromPipeline(Contract contract, Delegate pipeline) => throw new NotImplementedException();
        public ResolverOverride GetOverride<TMemberInfo, TDescriptor>(ref TDescriptor descriptor) where TDescriptor : IInjectionInfo<TMemberInfo> => throw new NotImplementedException();
        public object Get(Type type) => throw new NotImplementedException();
        public void Set(Type type, object policy) => throw new NotImplementedException();
        public void Clear(Type type) => throw new NotImplementedException();
        public void AsType(Type type) => throw new NotImplementedException();

        public IEnumerable<TSource> OfType<TSource>() where TSource : ISequenceSegment
        {
            throw new NotImplementedException();
        }

        public TSource FirstOrDefault<TSource>(Func<TSource, bool> predicate = null) where TSource : ISequenceSegment
        {
            throw new NotImplementedException();
        }

        ResolverOverride IBuilderContext.GetOverride<TMemberInfo, TDescriptor>(ref TDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}