using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext 
    {
        #region Resolution

        public object? Resolve(Type type, string? name)
        {
            var contract = new Contract(type, name);
            return Container.Resolve(ref contract, ref this);
        }

        #endregion


        #region Indirection

        public bool IsFaulted
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<RequestInfo>(_request.ToPointer()).IsFaulted;
                }
            }
        }

        public readonly ref Contract Contract
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<Contract>(_contract.ToPointer());
                }
            }
        }

        public readonly ref PipelineContext Parent
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<PipelineContext>(_parent.ToPointer());
                }
            }
        }

        public readonly ResolverOverride[] Overrides
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<RequestInfo>(_request.ToPointer()).Overrides;
                }
            }
        }

        public LifetimeManager? LifetimeManager => Registration as LifetimeManager;

        public ICollection<IDisposable> Scope => Container._scope.Disposables;

        #endregion


        #region Public Methods

        public void Error(string error)
        {
            unsafe
            {
                ref var info = ref Unsafe.AsRef<RequestInfo>(_request.ToPointer());

                info.Error = error;
                info.IsFaulted = true;
            }
        }

        #endregion


        #region Injection

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InjectionMember? GetInjected<TMemberInfo>() => Registration?.GetInjected<TMemberInfo>();


        #endregion


        #region Telemetry

        public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class 
            => new PipelineAction<TAction>(ref this, action);

        #endregion
    }
}
