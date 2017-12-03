using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Container;
using Unity.Container.Lifetime;
using Unity.Container.Registration;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.ObjectBuilder.BuildPlan;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Method;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Property;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.ObjectBuilder.Policies;
using Unity.ObjectBuilder.Strategies;
using Unity.Policy;
using Unity.Strategy;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private readonly UnityContainer _parent;
        private readonly ContainerContext _context;
        private readonly NamedTypesRegistry _registeredNames;
        private readonly List<UnityContainerExtension> _extensions;
        private readonly StagedStrategyChain<UnityBuildStage> _strategies;
        private readonly StagedStrategyChain<UnityBuildStage> _buildPlanStrategies;

        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        private LifetimeContainer _lifetimeContainer;

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
            _context = new ContainerContext(this);

            _extensions = new List<UnityContainerExtension>();
            _strategies = new StagedStrategyChain<UnityBuildStage>(_parent?._strategies);
            _registeredNames = new NamedTypesRegistry(_parent?._registeredNames);
            _buildPlanStrategies = new StagedStrategyChain<UnityBuildStage>(_parent?._buildPlanStrategies);
            _lifetimeContainer = new LifetimeContainer { _strategies, _buildPlanStrategies };

            _context.Set<IRegisteredNamesPolicy>(new RegisteredNamesPolicy(_registeredNames), typeof(UnityContainer));

            if (null == _parent) InitializeStrategies();

            RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }

        #endregion


        #region Default Strategies

        protected void InitializeStrategies()
        {
            var buildPlanCreatorPolicy = new DynamicMethodBuildPlanCreatorPolicy(_buildPlanStrategies);

            // Main strategy chain
            _strategies.AddNew<BuildKeyMappingStrategy>(UnityBuildStage.TypeMapping);
            _strategies.AddNew<LifetimeStrategy>(UnityBuildStage.Lifetime);
            _strategies.AddNew<ArrayResolutionStrategy>(UnityBuildStage.Creation);
            _strategies.AddNew<BuildPlanStrategy>(UnityBuildStage.Creation);

            // Build plan strategy chain
            _buildPlanStrategies.AddNew<DynamicMethodConstructorStrategy>(UnityBuildStage.Creation);
            _buildPlanStrategies.AddNew<DynamicMethodPropertySetterStrategy>(UnityBuildStage.Initialization);
            _buildPlanStrategies.AddNew<DynamicMethodCallStrategy>(UnityBuildStage.Initialization);

            // Policies - mostly used by the build plan strategies
            _context.SetDefault<IConstructorSelectorPolicy>(new DefaultUnityConstructorSelectorPolicy());
            _context.SetDefault<IPropertySelectorPolicy>(new DefaultUnityPropertySelectorPolicy());
            _context.SetDefault<IMethodSelectorPolicy>(new DefaultUnityMethodSelectorPolicy());
            _context.SetDefault<IBuildPlanCreatorPolicy>(buildPlanCreatorPolicy);

            _context.Set<IBuildPlanPolicy>(new DeferredResolveBuildPlanPolicy(), typeof(Func<>));
            _context.Set<ILifetimePolicy>(new PerResolveLifetimeManager(), typeof(Func<>));
            _context.Set<IBuildPlanCreatorPolicy>(new LazyDynamicMethodBuildPlanCreatorPolicy(), typeof(Lazy<>));
            _context.Set<IBuildPlanCreatorPolicy>(new EnumerableDynamicMethodBuildPlanCreatorPolicy(), typeof(IEnumerable<>));
        }

        #endregion


        #region Implementation

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


        private class PolicyListProxy : IPolicyList
        {
            private readonly IPolicyList _policies;
            private readonly IMap<Type, IBuilderPolicy> _registration;

            public PolicyListProxy(IPolicyList policies, IMap<Type, IBuilderPolicy> registration)
            {
                _policies = policies;
                _registration = registration;
            }

            public void Clear(Type policyInterface, object buildKey) { }

            public void ClearAll() { }

            public IBuilderPolicy Get(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
            {
                return _policies.Get(policyInterface, buildKey, out containingPolicyList);
            }

            public void Set(Type policyInterface, IBuilderPolicy policy, object buildKey = null)
            {
                _registration[policyInterface] = policy;
            }
        }
        


    }
}
