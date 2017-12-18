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
using Unity.ObjectBuilder.Strategies;
using Unity.Policy;
using Unity.Policy.BuildPlanCreator;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private readonly IPolicyStore _defaultPolicies;
        private readonly UnityContainer _parent;
        private readonly ContainerContext _context;
        private readonly LifetimeContainer _lifetimeContainer;
        private readonly List<UnityContainerExtension> _extensions;
        private readonly StagedStrategyChain<IBuilderStrategy, UnityBuildStage> _strategies;
        private readonly StagedStrategyChain<IBuilderStrategy, BuilderStage> _buildPlanStrategies;

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

            _context = new ContainerContext(this);

            _extensions = new List<UnityContainerExtension>();
            _strategies = new StagedStrategyChain<IBuilderStrategy, UnityBuildStage>(_parent?._strategies);
            _buildPlanStrategies = new StagedStrategyChain<IBuilderStrategy, BuilderStage>(_parent?._buildPlanStrategies);
            _lifetimeContainer = new LifetimeContainer { _strategies, _buildPlanStrategies };

            if (null == _parent) InitializeStrategies();

            // Default Policies
            _defaultPolicies = parent?._defaultPolicies ?? GetDefaultPolicies();
            this[null, null] = _defaultPolicies;

            // Register this instance
            RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }

        #endregion


        #region Defaults

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
        }


        private IPolicyStore GetDefaultPolicies()
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

        private static IPolicyStore CreateRegistration(Type type, string name)
        {
            return new InternalRegistration(type, name);
        }


        #endregion
    }
}
