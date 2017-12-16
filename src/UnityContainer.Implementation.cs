using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Container.Lifetime;
using Unity.Container.Storage;
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
using Unity.Policies.Default;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private readonly UnityContainer _parent;
        private readonly ContainerContext _context;
        private readonly List<UnityContainerExtension> _extensions;
        private readonly StagedStrategyChain<IBuilderStrategy, UnityBuildStage> _strategies;
        private readonly StagedStrategyChain<IBuilderStrategy, BuilderStage> _buildPlanStrategies;
        private readonly StagedStrategyChain<Func<UnityContainer, Type, string, IRegistration>, TypeSelectStage> _discovery;

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

            _discovery = parent?._discovery ?? GetTypeSelectStage();
            _extensions = new List<UnityContainerExtension>();
            _strategies = new StagedStrategyChain<IBuilderStrategy, UnityBuildStage>(_parent?._strategies);
            _buildPlanStrategies = new StagedStrategyChain<IBuilderStrategy, BuilderStage>(_parent?._buildPlanStrategies);
            _lifetimeContainer = new LifetimeContainer { _strategies, _buildPlanStrategies };


            if (null == _parent)
                InitializeStrategies();
            else
                this[null, null] = _parent[null, null];

            RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }

        #endregion


        #region Default Strategies

        protected void InitializeStrategies()
        {
            // Main strategy chain
            _strategies.Add(new BuildKeyMappingStrategy(), UnityBuildStage.TypeMapping);
            _strategies.Add(new LifetimeStrategy(), UnityBuildStage.Lifetime);
            _strategies.Add(new BuildPlanStrategy(), UnityBuildStage.Creation);

            // Build plan strategy chain
            _buildPlanStrategies.Add(new DynamicMethodConstructorStrategy(), BuilderStage.Creation);
            _buildPlanStrategies.Add(new DynamicMethodPropertySetterStrategy(), BuilderStage.Initialization);
            _buildPlanStrategies.Add(new DynamicMethodCallStrategy(), BuilderStage.Initialization);

            // Default Policies - mostly used by the build plan strategies
            this[null, null] = new LinkedMap<Type, IBuilderPolicy>(typeof(IResolverPolicy), new DefaultResolverPolicy())
            {
                [typeof(IBuildPlanCreatorPolicy)] = new DynamicMethodBuildPlanCreatorPolicy(_buildPlanStrategies),
                [typeof(IConstructorSelectorPolicy)] = new DefaultUnityConstructorSelectorPolicy(),
                [typeof(IPropertySelectorPolicy)] = new DefaultUnityPropertySelectorPolicy(),
                [typeof(IMethodSelectorPolicy)] = new DefaultUnityMethodSelectorPolicy()
            };

            // Special Cases
            this[typeof(Func<>), string.Empty, typeof(ILifetimePolicy)]  = new PerResolveLifetimeManager();
            this[typeof(Func<>), string.Empty, typeof(IBuildPlanPolicy)] = new DeferredResolveBuildPlanPolicy();
            this[typeof(Lazy<>), string.Empty, typeof(IBuildPlanCreatorPolicy)] = new LazyDynamicMethodBuildPlanCreatorPolicy();
            this[typeof(Array),  string.Empty, typeof(IBuildPlanCreatorPolicy)] = new ArrayBuildPlanCreatorPolicy();
            this[typeof(IEnumerable<>), string.Empty, typeof(IBuildPlanCreatorPolicy)] = new EnumerableDynamicMethodBuildPlanCreatorPolicy();
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


        private static IMap<Type, IBuilderPolicy> CreateRegistration(Type type, string name)
        {
            return new InternalRegistration(type, name);
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

            public IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
            {
                return _policies.Get(type, name, policyInterface, out list);
            }

            public void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
            {
                _registration[policyInterface] = policy;
            }

            public void Clear(Type type, string name, Type policyInterface)
            {
            }

            public void ClearAll()
            {
            }
        }
    }
}
