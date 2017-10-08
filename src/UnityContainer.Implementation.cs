using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Unity.Container.Properties;
using Microsoft.Practices.Unity;
using Unity.Builder;
using Unity.Events;
using Unity.Exceptions;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolve;
using Unity.Strategy;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private readonly UnityContainer _parent;

        private LifetimeContainer _lifetimeContainer;
        private StagedStrategyChain<UnityBuildStage> _strategies;
        private StagedStrategyChain<UnityBuildStage> _buildPlanStrategies;
        private PolicyList _policies;
        private NamedTypesRegistry _registeredNames;
        private List<UnityContainerExtension> _extensions;

        private IStrategyChain _cachedStrategies;
        private object _cachedStrategiesLock;

        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            _parent = parent;
            _parent?._lifetimeContainer.Add(this);

            InitializeBuilderState();
            if (null == _parent) InitializeStrategies();
            RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }

        #endregion


        #region Default Strategies

        protected void InitializeStrategies()
        {
            // Main strategy chain
            _strategies.AddNew<BuildKeyMappingStrategy>(UnityBuildStage.TypeMapping);
            _strategies.AddNew<HierarchicalLifetimeStrategy>(UnityBuildStage.Lifetime);
            _strategies.AddNew<LifetimeStrategy>(UnityBuildStage.Lifetime);

            _strategies.AddNew<ArrayResolutionStrategy>(UnityBuildStage.Creation);
            _strategies.AddNew<BuildPlanStrategy>(UnityBuildStage.Creation);

            // Build plan strategy chain
            _buildPlanStrategies.AddNew<DynamicMethodConstructorStrategy>(UnityBuildStage.Creation);
            _buildPlanStrategies.AddNew<DynamicMethodPropertySetterStrategy>(UnityBuildStage.Initialization);
            _buildPlanStrategies.AddNew<DynamicMethodCallStrategy>(UnityBuildStage.Initialization);

            // Policies - mostly used by the build plan strategies
            _policies.SetDefault<IConstructorSelectorPolicy>(new DefaultUnityConstructorSelectorPolicy());
            _policies.SetDefault<IPropertySelectorPolicy>(new DefaultUnityPropertySelectorPolicy());
            _policies.SetDefault<IMethodSelectorPolicy>(new DefaultUnityMethodSelectorPolicy());
            _policies.SetDefault<IBuildPlanCreatorPolicy>(new DynamicMethodBuildPlanCreatorPolicy(_buildPlanStrategies));
            _policies.Set<IBuildPlanPolicy>(new DeferredResolveBuildPlanPolicy(), typeof(Func<>));
            _policies.Set<ILifetimePolicy>(new PerResolveLifetimeManager(), typeof(Func<>));
            _policies.Set<IBuildPlanCreatorPolicy>( new LazyDynamicMethodBuildPlanCreatorPolicy(), typeof(Lazy<>));
        }


        private void SetLifetimeManager(Type lifetimeType, string name, LifetimeManager lifetimeManager)
        {
            if (lifetimeManager.InUse)
            {
                throw new InvalidOperationException(Resources.LifetimeManagerInUse);
            }

            if (lifetimeType.GetTypeInfo().IsGenericTypeDefinition)
            {
                LifetimeManagerFactory factory =
                    new LifetimeManagerFactory(new ExtensionContextImpl(this), lifetimeManager.GetType());
                _policies.Set<ILifetimeFactoryPolicy>(factory,
                    new NamedTypeBuildKey(lifetimeType, name));
            }
            else
            {
                lifetimeManager.InUse = true;
                _policies.Set<ILifetimePolicy>(lifetimeManager,
                    new NamedTypeBuildKey(lifetimeType, name));
                if (lifetimeManager is IDisposable)
                {
                    _lifetimeContainer.Add(lifetimeManager);
                }
            }
        }

        #endregion


        #region ObjectBuilder initialization

        private void InitializeBuilderState()
        {
            _registeredNames = new NamedTypesRegistry(ParentNameRegistry);
            _extensions = new List<UnityContainerExtension>();

            _lifetimeContainer = new LifetimeContainer();
            _strategies = new StagedStrategyChain<UnityBuildStage>(ParentStrategies);
            _buildPlanStrategies = new StagedStrategyChain<UnityBuildStage>(ParentBuildPlanStrategies);
            _policies = new PolicyList(ParentPolicies);
            _policies.Set<IRegisteredNamesPolicy>(new RegisteredNamesPolicy(_registeredNames), null);

            _cachedStrategies = null;
            _cachedStrategiesLock = new object();
        }

        private StagedStrategyChain<UnityBuildStage> ParentStrategies
        {
            get { return _parent == null ? null : _parent._strategies; }
        }

        private StagedStrategyChain<UnityBuildStage> ParentBuildPlanStrategies
        {
            get { return _parent == null ? null : _parent._buildPlanStrategies; }
        }

        private PolicyList ParentPolicies
        {
            get { return _parent == null ? null : _parent._policies; }
        }

        private NamedTypesRegistry ParentNameRegistry
        {
            get { return _parent == null ? null : _parent._registeredNames; }
        }

        #endregion


        #region Running ObjectBuilder

        private object DoBuildUp(Type t, string name, IEnumerable<ResolverOverride> resolverOverrides)
        {
            return DoBuildUp(t, null, name, resolverOverrides);
        }

        private object DoBuildUp(Type t, object existing, string name, IEnumerable<ResolverOverride> resolverOverrides)
        {
            IBuilderContext context = null;

            try
            {
                context = new BuilderContext(this,
                                             GetStrategies(),
                                             _lifetimeContainer,
                                             _policies,
                                             new NamedTypeBuildKey(t, name),
                                             existing);
                context.AddResolverOverrides(resolverOverrides);

                if (t.GetTypeInfo().IsGenericTypeDefinition)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture,
                        Resources.CannotResolveOpenGenericType,
                        t.FullName), nameof(t));
                }

                return context.Strategies.ExecuteBuildUp(context);
            }
            catch (Exception ex)
            {
                throw new ResolutionFailedException(t, name, ex, context);
            }
        }

        private IStrategyChain GetStrategies()
        {
            IStrategyChain buildStrategies = _cachedStrategies;
            if (buildStrategies == null)
            {
                lock (_cachedStrategiesLock)
                {
                    if (_cachedStrategies == null)
                    {
                        buildStrategies = _strategies.MakeStrategyChain();
                        _cachedStrategies = buildStrategies;
                    }
                    else
                    {
                        buildStrategies = _cachedStrategies;
                    }
                }
            }
            return buildStrategies;
        }

        #endregion


        #region Nested Types

        /// <summary>
        /// Implementation of the ExtensionContext that is actually used
        /// by the UnityContainer implementation.
        /// </summary>
        /// <remarks>
        /// This is a nested class so that it can access state in the
        /// container that would otherwise be inaccessible.
        /// </remarks>
        private class ExtensionContextImpl : ExtensionContext
        {
            private readonly UnityContainer container;

            public ExtensionContextImpl(UnityContainer container)
            {
                this.container = container;
            }

            public override IUnityContainer Container
            {
                get { return this.container; }
            }

            public override StagedStrategyChain<UnityBuildStage> Strategies
            {
                get { return this.container._strategies; }
            }

            public override StagedStrategyChain<UnityBuildStage> BuildPlanStrategies
            {
                get { return this.container._buildPlanStrategies; }
            }

            public override IPolicyList Policies
            {
                get { return this.container._policies; }
            }

            public override ILifetimeContainer Lifetime
            {
                get { return this.container._lifetimeContainer; }
            }

            public override void RegisterNamedType(Type t, string name)
            {
                this.container._registeredNames.RegisterType(t, name);
            }

            public override event EventHandler<RegisterEventArgs> Registering
            {
                add { this.container.Registering += value; }
                remove { this.container.Registering -= value; }
            }

            /// <summary>
            /// This event is raised when the <see cref="Unity.UnityContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
            /// or one of its overloads, is called.
            /// </summary>
            public override event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
            {
                add { this.container.RegisteringInstance += value; }
                remove { this.container.RegisteringInstance -= value; }
            }

            public override event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated
            {
                add { this.container.ChildContainerCreated += value; }
                remove { this.container.ChildContainerCreated -= value; }
            }
        }


        // Works like the ExternallyControlledLifetimeManager, but uses regular instead of weak references
        private class ContainerLifetimeManager : LifetimeManager
        {
            private object value;

            public override object GetValue()
            {
                return this.value;
            }

            public override void SetValue(object newValue)
            {
                this.value = newValue;
            }

            public override void RemoveValue()
            {
            }
        }

        #endregion

    }
}
