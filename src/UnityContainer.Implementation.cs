using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Container;
using Unity.Container.Lifetime;
using Unity.Container.Registration;
using Unity.Events;
using Unity.Exceptions;
using Unity.Extension;
using Unity.Lifetime;
using Unity.ObjectBuilder;
using Unity.ObjectBuilder.BuildPlan;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Method;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Property;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.ObjectBuilder.Policies;
using Unity.ObjectBuilder.Strategies;
using Unity.Policy;
using Unity.Resolution;
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
            _policies.Set<IBuildPlanCreatorPolicy>(new LazyDynamicMethodBuildPlanCreatorPolicy(), typeof(Lazy<>));
            _policies.Set<IBuildPlanCreatorPolicy>(new EnumerableDynamicMethodBuildPlanCreatorPolicy(), typeof(IEnumerable<>));
        }


        private void SetLifetimeManager(Type lifetimeType, string name, LifetimeManager lifetimeManager)
        {
            if (lifetimeManager.InUse)
            {
                throw new InvalidOperationException(Constants.LifetimeManagerInUse);
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
                        String.Format(CultureInfo.CurrentCulture,
                            Constants.CannotResolveOpenGenericType,
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


        #region Implementation

        /// <summary>
        /// Verifies that an argument instance is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy, or instance can be 
        /// assigned through a runtime wrapper, as is the case for COM Objects).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentInstance">The instance that will be assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        private static void InstanceIsAssignable(Type assignmentTargetType, object assignmentInstance, string argumentName)
        {
            if (!(assignmentTargetType ?? throw new ArgumentNullException(nameof(assignmentTargetType)))
                .GetTypeInfo().IsAssignableFrom((assignmentInstance ?? throw new ArgumentNullException(nameof(assignmentInstance))).GetType().GetTypeInfo()))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Constants.TypesAreNotAssignable,
                        assignmentTargetType, GetTypeName(assignmentInstance)),
                    argumentName);
            }
        }

        private static string GetTypeName(object assignmentInstance)
        {
            string assignmentInstanceType;
            try
            {
                assignmentInstanceType = assignmentInstance.GetType().FullName;
            }
            catch (Exception)
            {
                assignmentInstanceType = Constants.UnknownType;
            }

            return assignmentInstanceType;
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
            private readonly UnityContainer _container;

            public ExtensionContextImpl(UnityContainer container)
            {
                _container = container ?? throw new ArgumentNullException(nameof(container));
            }

            public override IUnityContainer Container => _container;

            public override IStagedStrategyChain<UnityBuildStage> Strategies => _container._strategies;

            public override IStagedStrategyChain<UnityBuildStage> BuildPlanStrategies => _container._buildPlanStrategies;

            public override IPolicyList Policies => _container._policies;

            public override ILifetimeContainer Lifetime => _container._lifetimeContainer;

            public override event EventHandler<RegisterEventArgs> Registering
            {
                add => _container.Registering += value;
                remove => _container.Registering -= value;
            }

            /// <summary>
            /// This event is raised when the <see cref="Unity.UnityContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
            /// or one of its overloads, is called.
            /// </summary>
            public override event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
            {
                add => _container.RegisteringInstance += value;
                remove => _container.RegisteringInstance -= value;
            }

            public override event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated
            {
                add => _container.ChildContainerCreated += value;
                remove => _container.ChildContainerCreated -= value;
            }
        }


        // Works like the ExternallyControlledLifetimeManager, but uses regular instead of weak references
        private class ContainerLifetimeManager : LifetimeManager
        {
            private object _value;

            public override object GetValue()
            {
                return _value;
            }

            public override void SetValue(object newValue)
            {
                _value = newValue;
            }

            public override void RemoveValue()
            {
            }
        }

        #endregion
    }
}
