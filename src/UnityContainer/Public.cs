using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Unity.Events;
using Unity.Extension;
using Unity.Factories;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Storage;
using Unity.Utility;

namespace Unity
{
    [Flags]
    public enum ModeFlags
    {
        Optimized = 0,

        Diagnostic = 0x00000001,
        Activated  = 0x00000010,
        Compiled   = 0x00000100,
        Legacy     = 0x00001000,
    }


    public partial class UnityContainer
    {
        #region Constants

        // The default timeout for pipeline lock
        public static int DefaultTimeOut = Timeout.Infinite;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer(ModeFlags mode = ModeFlags.Optimized)
        {
            if (!mode.IsValid()) throw new ArgumentException("'Activated' and 'Diagnostic' flags are mutually exclusive.");

            /////////////////////////////////////////////////////////////
            // Initialize Root 
            _root = this;
            ExecutionMode = mode;
            LifetimeContainer = new LifetimeContainer(this);
            Register = AddOrReplace;
            
            //Built-In Registrations

            // Defaults
            Defaults = new DefaultPolicies(this);

            // IUnityContainer, IUnityContainerAsync
            var container = new ExplicitRegistration(this, null, typeof(UnityContainer), new ContainerLifetimeManager())
            {
                Pipeline = (ref PipelineContext c) => c.Container
            };
            Debug.Assert(null != container.LifetimeManager);

            // Create Registries
            _metadata  = new Metadata();
            _registry  = new Registry(Defaults);
            _registry.Set(typeof(IUnityContainer),      null, container);  // TODO: Remove redundancy
            _registry.Set(typeof(IUnityContainerAsync), null, container);
            _registry.Set(typeof(IUnityContainer),      null, container.LifetimeManager.Pipeline);
            _registry.Set(typeof(IUnityContainerAsync), null, container.LifetimeManager.Pipeline);

            /////////////////////////////////////////////////////////////
            // Built-In Features

            var func = new PolicySet(this);
            _registry.Set(typeof(Func<>), func);  
                 func.Set(typeof(LifetimeManager),     new PerResolveLifetimeManager());
                 func.Set(typeof(TypeFactoryDelegate), FuncResolver.Factory);                                                         // Func<> Factory
            _registry.Set(typeof(Lazy<>),              new PolicySet(this, typeof(TypeFactoryDelegate), LazyResolver.Factory));       // Lazy<>
            _registry.Set(typeof(IEnumerable<>),       new PolicySet(this, typeof(TypeFactoryDelegate), EnumerableResolver.Factory)); // Enumerable
            _registry.Set(typeof(IRegex<>),            new PolicySet(this, typeof(TypeFactoryDelegate), RegExResolver.Factory));      // Regular Expression Enumerable

            
            /////////////////////////////////////////////////////////////
            // Pipelines

            var factory  = new FactoryPipeline();
            var lifetime = new LifetimePipeline();

            // Mode of operation
            if (ExecutionMode.IsDiagnostic())
            {
                /////////////////////////////////////////////////////////////
                // Setup Diagnostic mode

                var diagnostic = new DiagnosticPipeline();

                // Create Context
                Context = new ContainerContext(this,
                    new StagedStrategyChain<Pipeline, Stage> // Type Build Pipeline
                    {
                        { diagnostic,                      Stage.Diagnostic },
                        { factory,                         Stage.Factory },
                        { new MappingDiagnostic(),         Stage.TypeMapping },
                        { new ConstructorDiagnostic(this), Stage.Creation },
                        { new FieldDiagnostic(this),       Stage.Fields },
                        { new PropertyDiagnostic(this),    Stage.Properties },
                        { new MethodDiagnostic(this),      Stage.Methods }
                    },
                    new StagedStrategyChain<Pipeline, Stage> // Factory Resolve Pipeline
                    {
                        { diagnostic,                      Stage.Diagnostic },
                        { factory,                         Stage.Factory }
                    },
                    new StagedStrategyChain<Pipeline, Stage> // Instance Resolve Pipeline
                    {
                        { diagnostic,                      Stage.Diagnostic },
                        { factory,                         Stage.Factory }
                    });

                // Build process
                DependencyResolvePipeline = ValidatingDependencyResolvePipeline;

                // Validators
                ValidateType  = DiagnosticValidateType;
                ValidateTypes = DiagnosticValidateTypes;
                CreateErrorMessage = CreateDiagnosticMessage;

            }
            else
            {
                // Create Context
                Context = new ContainerContext(this,
                    new StagedStrategyChain<Pipeline, Stage> // Type Build Pipeline
                    {
                        { lifetime,                      Stage.Lifetime },
                        { factory,                       Stage.Factory },
                        { new MappingPipeline(),         Stage.TypeMapping },
                        { new ConstructorPipeline(this), Stage.Creation },
                        { new FieldPipeline(this),       Stage.Fields },
                        { new PropertyPipeline(this),    Stage.Properties },
                        { new MethodPipeline(this),      Stage.Methods }
                    },
                    new StagedStrategyChain<Pipeline, Stage> // Factory Resolve Pipeline
                    {
                        { lifetime,                      Stage.Lifetime },
                        { factory,                       Stage.Factory }
                    },
                    new StagedStrategyChain<Pipeline, Stage> // Instance Resolve Pipeline
                    {
                        { factory,                       Stage.Factory }
                    });
            }

            
            /////////////////////////////////////////////////////////////
            // Build Mode

            var build = _root.ExecutionMode.BuildMode();

            PipelineFromRegistration = build switch
            {
                ModeFlags.Activated => PipelineFromRegistrationActivated,
                ModeFlags.Compiled  => PipelineFromRegistrationCompiled,
                _ => (FromRegistration)PipelineFromRegistrationOptimized
            };

            PipelineFromUnregisteredType = build switch
            {
                ModeFlags.Activated => PipelineFromUnregisteredTypeActivated,
                ModeFlags.Compiled  => PipelineFromUnregisteredTypeCompiled,
                _ => (FromUnregistered)PipelineFromUnregisteredTypeOptimized
            };

            PipelineFromOpenGeneric = build switch
            {
                ModeFlags.Activated => PipelineFromOpenGenericActivated,
                ModeFlags.Compiled  => PipelineFromOpenGenericCompiled,
                _ => (FromOpenGeneric) PipelineFromOpenGenericOptimized
            };
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            var key = new HashKey(type, name);

            // Iterate through hierarchy and check if exists
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registry
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry;
                var targetBucket = key.HashCode % registry.Buckets.Length;

                // Look for exact match
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key || !candidate.IsExplicit) continue;

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerable<IContainerRegistration> Registrations
        {
            get
            {
                var set = new QuickSet();
                IContainerRegistration cashe;

                // First, add the built-in registrations
                Debug.Assert(null != _root._registry);
                set.Add(ref _root._registry.Entries[1].Key);
                set.Add(ref _root._registry.Entries[2].Key);

                // IUnityContainer & IUnityContainerAsync
                yield return _root._registry.Entries[1].Cache;
                yield return _root._registry.Entries[2].Cache;

                // Explicit registrations
                for (UnityContainer? container = this; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._metadata) continue;

                    // Hold on to registries
                    Debug.Assert(null != container._registry);
                    var registry = container._registry;
                    for (var i = 0; i < registry.Count; i++)
                    {
                        if (!registry.Entries[i].IsExplicit || !set.Add(ref registry.Entries[i].Key))
                            continue;

                        cashe = registry.Entries[i].Cache;
                        
                        // Create wrapper is required
                        if (null == cashe)
                        {
                            var type = registry.Entries[i].Key.Type;
                            Debug.Assert(null != type);

                            cashe = new RegistrationWrapper(type, registry.Entries[i].Policies);
                            registry.Entries[i].Cache = cashe;
                        }

                        yield return cashe;
                    }
                }
            }
        }

        #endregion


        #region Child container management

        /// <inheritdoc />
        public UnityContainer CreateChildContainer()
        {
            var child = new UnityContainer(this);
            ChildContainerCreated?.Invoke(this, new ChildContainerCreatedEventArgs(child.Context));
            return child;
        }

        /// <inheritdoc />
        UnityContainer? Parent => _parent;

        #endregion


        #region Extension Management

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public UnityContainer AddExtension(IUnityContainerExtensionConfigurator extension)
        {
            lock (LifetimeContainer)
            {
                if (null == _extensions)
                    _extensions = new List<IUnityContainerExtensionConfigurator>();

                _extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
            }
            (extension as UnityContainerExtension)?.InitializeExtension(Context);

            return this;
        }

        /// <summary>
        /// Resolve access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public object? Configure(Type configurationInterface)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            return _extensions?.FirstOrDefault(ex => configurationInterface.GetTypeInfo()
                                                                           .IsAssignableFrom(ex.GetType()
                                                                           .GetTypeInfo()));
#else
            return _extensions?.FirstOrDefault(ex => configurationInterface.IsAssignableFrom(ex.GetType()));
#endif
        }

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// Disposing the container also disposes any child containers,
        /// and disposes any instances whose lifetimes are managed
        /// by the container.
        /// </remarks>
        public void Dispose()
        {
            List<Exception>? exceptions = null;

            try
            {
                _parent?.LifetimeContainer.Remove(this);
                LifetimeContainer.Dispose();
            }
            catch (Exception exception)
            {
                exceptions = new List<Exception> { exception };
            }

            if (null != _extensions)
            {
                foreach (IDisposable disposable in _extensions.OfType<IDisposable>()
                                                              .ToArray())
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (null == exceptions) exceptions = new List<Exception>();
                        exceptions.Add(e);
                    }
                }

                _extensions = null;
            }

            if (null != exceptions && exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            else if (null != exceptions && exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }
        }

        #endregion
    }
}
