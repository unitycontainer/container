using System;
using System.Collections.Generic;
using Unity;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

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
        public Type TypeDefinition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object Target { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Type TargetType => throw new NotImplementedException();

        public object Capture(Exception exception) => throw new NotImplementedException();
        public object Resolve(Type type, string name) => throw new NotImplementedException();
        public object MapTo(Contract contract) => throw new NotImplementedException();
        public object Resolve(Contract contract) => throw new NotImplementedException();
        public object Resolve(Contract contract, ref ErrorDescriptor errorInfo) => throw new NotImplementedException();
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

        public void Resolve<TMemberInfo>(ref InjectionInfoStruct<TMemberInfo> info)
        {
            throw new NotImplementedException();
        }

        void IBuildPlanContext.Error(string error)
        {
            throw new NotImplementedException();
        }

        public object FromPipeline<TMember>(TMember member, ref Contract contract, Delegate pipeline)
        {
            throw new NotImplementedException();
        }

        public object Resolve<TMember>(TMember member, ref Contract contract, object value)
        {
            throw new NotImplementedException();
        }

        public object Resolve<TMemberInfo>(TMemberInfo member, ref Contract contract)
        {
            throw new NotImplementedException();
        }

        public object Resolve<TMember>(TMember member, ref Contract contract, ResolveDelegate<BuilderContext> pipeline)
        {
            throw new NotImplementedException();
        }

        public object ResolveOptional<TMemberInfo>(TMemberInfo member, ref Contract contract)
        {
            throw new NotImplementedException();
        }

        public ResolverOverride GetOverride<TMemberInfo>(TMemberInfo info, ref Contract contract)
        {
            throw new NotImplementedException();
        }
    }
}
