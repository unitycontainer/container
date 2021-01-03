using System;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        #region Property

        /// <inheritdoc/>
        public IPolicies Policies => Container.Policies;

        #endregion


        #region Resolution

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Resolve()
        {
            Target = Container.Resolve(ref this);
            return Target;
        }

        public object? Resolve(Type type, string? name)
        {
            var contract = new Contract(type, name);
            var context = CreateContext(ref contract);

            Target = Container.Resolve(ref context);

            return Target;
        }

        #endregion


        #region Indirection

        public object? Target
        {
            get => _target;
            set
            {
                _target = value;

                unsafe
                {
                    if ((_perResolve || Registration is Lifetime.PerResolveLifetimeManager) && 
                        !ReferenceEquals(value, UnityContainer.NoValue))
                    {
                        ref var contract = ref Unsafe.AsRef<Contract>(_registration.ToPointer());
                        RequestInfo.PerResolve = new PerResolveOverride(in contract, value);
                    }
                }
            }
        }
        
        public object? Existing { get => _target; set => _target = value; }

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

        private readonly ref RequestInfo RequestInfo
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<RequestInfo>(_request.ToPointer());
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

        #endregion


        #region Public Methods

        public object Error(string error)
        {
            unsafe
            {
                return Unsafe.AsRef<ErrorInfo>(_error.ToPointer())
                             .Error(error);
            }
        }

        public object Throw(Exception exception)
        {
            unsafe
            {
                return Unsafe.AsRef<ErrorInfo>(_error.ToPointer())
                             .Throw(exception);
            }
        }

        public object Capture(Exception exception)
        {
            unsafe
            {
                return Unsafe.AsRef<ErrorInfo>(_error.ToPointer())
                             .Capture(exception);
            }
        }

        public object? GetValueRecursively<TInfo>(TInfo info, object? value)
        {
            return value switch
            {
                ResolveDelegate<PipelineContext> resolver => GetValueRecursively(info, resolver(ref this)),

                IResolve iResolve                         => GetValueRecursively(info, iResolve.Resolve(ref this)),

                IResolverFactory<TInfo> infoFactory       => GetValueRecursively(info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                       .Invoke(ref this)),
                IResolverFactory<Type> typeFactory        => GetValueRecursively(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                                                       .Invoke(ref this)),
                _ => value,
            };
        }

        internal void Reset()
        {
            _target = null;
            Registration = null;
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
        public PipelineContext CreateContext(UnityContainer container, ref Contract contract)
            => new PipelineContext(container, ref contract, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(ref Contract contract, ref ErrorInfo error)
            => new PipelineContext(ref contract, ref error, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext CreateContext(ref Contract contract)
            => new PipelineContext(ref contract, ref this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineContext Map(ref Contract contract)
            => new PipelineContext(ref contract, ref this, Registration is Lifetime.PerResolveLifetimeManager);

        #endregion


        #region Scope

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Scope WithContainer(UnityContainer container)
            => new Scope(container, ref this);
        
        #endregion
    }
}
