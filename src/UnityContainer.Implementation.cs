using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Container.Lifetime;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Method;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Property;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;
using Unity.Policy.BuildPlanCreator;
using Unity.Registration;
using Unity.Storage;
using Unity.Strategies;
using Unity.Strategy;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        // Container specific
        private readonly UnityContainer _parent;
        private readonly LifetimeContainer _lifetimeContainer;
        private readonly List<UnityContainerExtension> _extensions;
        
        // Policies
        private readonly IPolicySet _defaultPolicies;
        private readonly ContainerContext _context;
        
        // Strategies
        private readonly StagedStrategyChain<IBuilderStrategy, UnityBuildStage> _strategies;
        private readonly StagedStrategyChain<IBuilderStrategy, BuilderStage> _buildPlanStrategies;
        
        // Events
        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;
        
        // Caches
        private IRegisterTypeStrategy[] _registerTypeStrategies;
        private IStrategyChain _strategyChain;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            // Parent
            _parent = parent;
            _parent?._lifetimeContainer.Add(this);

            // Strategies
            _strategies = new StagedStrategyChain<IBuilderStrategy, UnityBuildStage>(_parent?._strategies);
            _buildPlanStrategies = new StagedStrategyChain<IBuilderStrategy, BuilderStage>(_parent?._buildPlanStrategies);

            // Lifetime
            _lifetimeContainer = new LifetimeContainer(this) { _strategies, _buildPlanStrategies };

            // Default Policies
            if (null == _parent) InitializeRootContainer();
            _defaultPolicies = parent?._defaultPolicies ?? GetDefaultPolicies();
            this[null, null] = _defaultPolicies;

            // Context and policies
            _extensions = new List<UnityContainerExtension>();
            _context = new ContainerContext(this);

            // Caches
            OnStrategiesChanged(this, null);
            _strategies.Invalidated += OnStrategiesChanged;
        }

        #endregion


        #region Defaults

        protected void InitializeRootContainer()
        {
            // Main strategy chain
            _strategies.Add(new BuildKeyMappingStrategy(), UnityBuildStage.TypeMapping);
            _strategies.Add(new LifetimeStrategy(), UnityBuildStage.Lifetime);
            _strategies.Add(new BuildPlanStrategy(), UnityBuildStage.Creation);

            // Build plan strategy chain
            _buildPlanStrategies.Add(new DynamicMethodConstructorStrategy(), BuilderStage.Creation);
            _buildPlanStrategies.Add(new DynamicMethodPropertySetterStrategy(), BuilderStage.Initialization);
            _buildPlanStrategies.Add(new DynamicMethodCallStrategy(), BuilderStage.Initialization);

            // Special Cases
            this[null, null] = _defaultPolicies;
            this[typeof(Func<>), string.Empty, typeof(ILifetimePolicy)]         = new PerResolveLifetimeManager();
            this[typeof(Func<>), string.Empty, typeof(IBuildPlanPolicy)]        = new DeferredResolveCreatorPolicy();
            this[typeof(Lazy<>), string.Empty, typeof(IBuildPlanCreatorPolicy)] = new GenericLazyBuildPlanCreatorPolicy();
            this[typeof(Array),  string.Empty, typeof(IBuildPlanCreatorPolicy)] = 
                new DelegateBasedBuildPlanCreatorPolicy(typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveArray)), 
                                                        context => context.OriginalBuildKey.Type.GetElementType());
            this[typeof(IEnumerable<>), string.Empty, typeof(IBuildPlanCreatorPolicy)] = 
                new DelegateBasedBuildPlanCreatorPolicy(typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveEnumerable)),
                                                        context => context.BuildKey.Type.GetTypeInfo().GenericTypeArguments.First());
            // Register this instance
            RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }


        private IPolicySet GetDefaultPolicies()
        {
            var defaults = new InternalRegistration(null, null);

            defaults.Set(typeof(IConstructorSelectorPolicy), new DefaultUnityConstructorSelectorPolicy());
            defaults.Set(typeof(IPropertySelectorPolicy),    new DefaultUnityPropertySelectorPolicy());
            defaults.Set(typeof(IMethodSelectorPolicy),      new DefaultUnityMethodSelectorPolicy());
            defaults.Set(typeof(IBuildPlanCreatorPolicy),    new DynamicMethodBuildPlanCreatorPolicy(_buildPlanStrategies));

            return defaults;
        }

        #endregion


        #region Implementation

        private void OnStrategiesChanged(object sender, EventArgs e)
        {
            _registerTypeStrategies = _strategies.OfType<IRegisterTypeStrategy>().ToArray();
            _strategyChain = new StrategyChain(_strategies);
        }

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

        private static IPolicySet CreateRegistration(Type type, string name)
        {
            return new InternalRegistration(type, name);
        }

        private UnityContainer GetRootContainer()
        {
            UnityContainer container;

            for (container = this; container._parent != null; container = container._parent) ;

            return container;
        }

        #endregion
    }
}
