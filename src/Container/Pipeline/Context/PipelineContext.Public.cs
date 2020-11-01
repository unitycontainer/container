using System;
using System.Runtime.CompilerServices;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext 
    {
        #region Resolution

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Resolve() => Container.Resolve(ref this);

        public object? Resolve(Type type, string? name)
        {
            var contract = new Contract(type, name);
            var context = CreateContext(ref contract);

            Target = Container.Resolve(ref context);

            return Target;
        }

        #endregion


        #region Indirection

        public bool IsFaulted
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ErrorInfo>(_error.ToPointer()).IsFaulted;
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

        public readonly ref ErrorInfo ErrorInfo
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<ErrorInfo>(_error.ToPointer());
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

        public Defaults Defaults => Container._policies;

        #endregion


        #region Public Methods

        public object Error(string error)
        {
            unsafe
            {
                ref var info = ref Unsafe.AsRef<ErrorInfo>(_error.ToPointer());

                info.IsFaulted = true;
                info.Message = error;
            }

            return error;
        }

        public void Exception(Exception exception)
        {
            unsafe
            {
                ref var info = ref Unsafe.AsRef<ErrorInfo>(_error.ToPointer());

                info.Message   = exception.Message;
                info.Exception = exception;
                info.IsFaulted = true;
            }
        }

        #endregion


        #region Telemetry

        public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class 
            => new PipelineAction<TAction>(ref this, action);

        #endregion


        #region Child Context

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(UnityContainer container, ref Contract contract, RegistrationManager manager)
            => new PipelineContext(container, ref contract, manager, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(ref Contract contract, ref ErrorInfo error)
            => new PipelineContext(ref contract, ref error, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(ref Contract contract)
            => new PipelineContext(ref contract, ref this);

        #endregion
    }
}
