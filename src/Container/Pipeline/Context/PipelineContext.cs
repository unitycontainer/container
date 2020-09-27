using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {Type},  Name: {Name}")]
    public partial struct PipelineContext
    {
        #region Fields

        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _contract;

        public readonly UnityContainer Container;
        public readonly RegistrationManager? Registration;

        #endregion


        #region Constructors

        public PipelineContext(UnityContainer container, ref Contract contract, object existing, RegistrationManager manager, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target   = existing;
            Action = manager.Data!;
            
            Registration = manager;
            Container = container;
        }

        public PipelineContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;
            
            Registration = manager;
            Container = container;
        }

        public PipelineContext(ref PipelineContext parent, object action, object? data = null)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _request = parent._request;
                _contract = parent._contract;
            }

            Target = data;
            Action = action;

            Registration = parent.Registration;
            Container = parent.Container;
        }


        private PipelineContext(ref PipelineContext parent, ref Contract contract, object? action)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = action;
            
            Registration = parent.Registration;
            Container = parent.Container;
        }

        #endregion


        #region Public Properties


        public object? Action { get; set; }

        public object? Target { get; set; }

        #endregion


        #region Inderection

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

        public LifetimeManager? LifetimeManager => Registration as LifetimeManager;
        
        public ICollection<IDisposable> Scope => Container._scope.Disposables;

        #endregion


        #region Public Methods


        public void Error(string error)
        {
            unsafe
            {
                ref var info = ref Unsafe.AsRef<RequestInfo>(_request.ToPointer());

                info.Error     = error;
                info.IsFaulted = true;
            }
        }

        public object? Resolve(Type type)
        {
            throw new NotImplementedException();
            //

            //            // Process overrides if any
            //            if (0 < Overrides.Length)
            //            {
            //                NamedType namedType = new NamedType
            //                {
            //                    Type = type,
            //                    Name = Name
            //                };

            //                // Check if this parameter is overridden
            //                for (var index = Overrides.Length - 1; index >= 0; --index)
            //                {
            //                    var resolverOverride = Overrides[index];
            //                    // If matches with current parameter
            //                    if (resolverOverride is IResolve resolverPolicy &&
            //                        resolverOverride is IEquatable<NamedType> comparer && comparer.Equals(namedType))
            //                    {
            //                        var context = this;

            //                        return DependencyResolvePipeline(ref context, resolverPolicy.Resolve);
            //                    }
            //                }
            //            }

            //            var pipeline = ContainerContext.Container.GetPipeline(type, Name);
            //            LifetimeManager? manager = pipeline.Target as LifetimeManager;

            //            // Check if already created and acquire a lock if not
            //            if (manager is PerResolveLifetimeManager)
            //            {
            //                manager = List.Get(type, Name, typeof(LifetimeManager)) as LifetimeManager ?? manager;
            //            }

            //            if (null != manager)
            //            {
            //                // Make blocking check for result
            //                var value = manager.Get(LifetimeContainer);
            //                if (LifetimeManager.NoValue != value) return value;

            //                if (null != manager.Scope) 
            //                {
            //                    var scope = ContainerContext;
            //                    try
            //                    {
            //                        ContainerContext = (ContainerContext)manager.Scope;
            //                        return Resolve(type, null, pipeline);
            //                    }
            //                    finally
            //                    {
            //                        ContainerContext = scope;
            //                    }
            //                }
            //            }

            //            return Resolve(type, null, pipeline);
        }

        public object? Resolve(Type type, string? name, ResolveDelegate<PipelineContext> pipeline)
        {
            throw new NotImplementedException();
            //

//            var thisContext = this;
//            var manager = pipeline.Target as LifetimeManager;
//            object? value;

//            unsafe
//            {
//                if (type != Type || name != Name)
//                {
//                    // Setup Context
//                    var context = new PipelineContext
//                    {
//                        List = List,
//                        Type = type,
//                        Name = name,
//                        Overrides = Overrides,
//                        DeclaringType = Type,
//                        ContainerContext = null != manager?.Scope
//                                 ? (ContainerContext)manager.Scope
//                                 : ContainerContext,
//#if !NET40
//                        Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
//#endif
//                    };
//                    value = pipeline(ref context);
//                }
//                else
//                    value = pipeline(ref thisContext);

//                manager?.SetValue(value, LifetimeContainer);
//                return value;
//            }

        }

        #endregion
    }
}
