using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Abstracts;
using Unity.Builder;
using Unity.Events;
using Unity.Extension;
using Unity.Extensions;
using Unity.Factories;
using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constructors

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
        {
            /////////////////////////////////////////////////////////////
            // Initialize Root 

            _root = this;

            // Create storage
            Defaults = new DefaultPolicies(this);
            LifetimeContainer = new LifetimeContainer(this);

            // Create Registry and set Factory strategy
            _metadata = new Registry<int[]>();
            _registry = new Registry<IPolicySet>(Defaults);

            //Built-In Registrations

            // Register Container as IUnityContainer & IUnityContainerAsync
            var container = new ImplicitRegistration(this, null, (ref BuilderContext c) => c.Container);
            _registry.Set(typeof(IUnityContainer), null, container);
            _registry.Set(typeof(IUnityContainerAsync), null, container);

            // Built-In Features
            var func = new PolicySet(this);
            _registry.Set(typeof(Func<>), func);  
                 func.Set(typeof(LifetimeManager),     new PerResolveLifetimeManager());
                 func.Set(typeof(TypeResolverFactory), FuncResolver.Factory);                                                   // Func<> Factory
            _registry.Set(typeof(Lazy<>),        new PolicySet(this, typeof(TypeResolverFactory), LazyResolver.Factory));       // Lazy<>
            _registry.Set(typeof(IEnumerable<>), new PolicySet(this, typeof(TypeResolverFactory), EnumerableResolver.Factory)); // Enumerable
            _registry.Set(typeof(IRegex<>),      new PolicySet(this, typeof(TypeResolverFactory), RegExResolver.Factory));      // Regular Expression Enumerable

            /////////////////////////////////////////////////////////////
            // Setup Pipelines

            var lifetimeBuilder = new LifetimeBuilder();
            var factoryBuilder = new FactoryBuilder();

            // Create Local Context
            Context = new ContainerContext(this,
                new StagedStrategyChain<PipelineBuilder, PipelineStage> // Type Build Pipeline
                {
                    { lifetimeBuilder,              PipelineStage.Lifetime },
                    { new MappingBuilder(),         PipelineStage.TypeMapping },
                    { factoryBuilder,               PipelineStage.Factory },
                    { new ConstructorBuilder(this), PipelineStage.Creation },
                    { new FieldBuilder(this),       PipelineStage.Fields },
                    { new PropertyBuilder(this),    PipelineStage.Properties },
                    { new MethodBuilder(this),      PipelineStage.Methods }
                },
                new StagedStrategyChain<PipelineBuilder, PipelineStage> // Factory Resolve Pipeline
                {
                    { lifetimeBuilder, PipelineStage.Lifetime },
                    { factoryBuilder,  PipelineStage.Factory }
                },
                new StagedStrategyChain<PipelineBuilder, PipelineStage> // Instance Resolve Pipeline
                {
                    { lifetimeBuilder, PipelineStage.Lifetime },
                });

            /////////////////////////////////////////////////////////////
            // Container Specific

            Register = AddOrReplace;
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            int hashCode = NamedType.GetHashCode(type, name);

            // Iterate through hierarchy and check if exists
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registry
                if (null == container._metadata || null == container._registry)
                    continue;

                // Look for exact match
                var registry = container._registry;
                var targetBucket = (hashCode & HashMask) % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Type != type ||
                      !(candidate.Value is ImplicitRegistration set) || set.Name != name)
                        continue;

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
                var set = new QuickSet<Type>();
                var registry = _root._registry ?? throw new InvalidOperationException();

                // First, add the built-in registrations
                set.Add(registry.Entries[1].HashCode, typeof(IUnityContainer));
                set.Add(registry.Entries[2].HashCode, typeof(IUnityContainerAsync));


                // IUnityContainer
                yield return new ContainerRegistrationStruct
                {
                    RegisteredType = typeof(IUnityContainer),
                    MappedToType = typeof(UnityContainer),
                    LifetimeManager = TransientLifetimeManager.Instance
                };

                // IUnityContainerAsync
                yield return new ContainerRegistrationStruct
                {
                    RegisteredType = typeof(IUnityContainerAsync),
                    MappedToType = typeof(UnityContainer),
                    LifetimeManager = TransientLifetimeManager.Instance
                };
                
                
                // Explicit registrations
                for (UnityContainer? container = this; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._metadata || null == container._registry)
                        continue;

                    // Hold on to registries
                    registry = container._registry;
                    for (var i = 0; i < registry.Count; i++)
                    {
                        var type = registry.Entries[i].Type;
                        var registration = registry.Entries[i].Value as ExplicitRegistration;

                        if (null == registration || !set.Add(registry.Entries[i].HashCode, type))
                            continue;

                        yield return new ContainerRegistrationStruct
                        {
                            RegisteredType = type,
                            Name = registration.Name,
                            MappedToType = registration.Type,
                            LifetimeManager = registration.LifetimeManager ?? TransientLifetimeManager.Instance,
                        };
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
