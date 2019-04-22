using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Events;
using Unity.Extension;
using Unity.Extensions;
using Unity.Factories;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Processors;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    [CLSCompliant(true)]
    public partial class UnityContainer
    {
        #region Fields

        // Container specific
        private readonly UnityContainer _root;
        private readonly UnityContainer _parent;
        internal readonly LifetimeContainer LifetimeContainer;
        private static readonly ContainerLifetimeManager _containerManager = new ContainerLifetimeManager();
        private List<IUnityContainerExtensionConfigurator> _extensions;

        // Policies
        private readonly ContainerContext _context;

        // Strategies
        private StagedStrategyChain<BuilderStrategy, UnityBuildStage> _strategies;
        private StagedStrategyChain<MemberProcessor, BuilderStage> _processors;

        // Caches
        private BuilderStrategy[] _strategiesChain;
        private MemberProcessor[] _processorsChain;

        // Events
        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        #endregion


        #region Constructor

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            // Parent
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _parent.LifetimeContainer.Add(this);

            // Root of the hierarchy
            _root = _parent._root;

            // Lifetime
            LifetimeContainer = new LifetimeContainer(this);

            // Registry
            Register = InitAndAdd;

            /////////////////////////////////////////////////////////////

            SetDefaultPolicies = parent.SetDefaultPolicies;

            // Context and policies
            _context = new ContainerContext(this);

            // Strategies
            _strategies = _parent._strategies;
            _strategiesChain = _parent._strategiesChain;
            _strategies.Invalidated += OnStrategiesChanged;

            // Caches
            SetDefaultPolicies(this);
        }

        #endregion


        #region Default Policies

        private void InitializeRootRegistry()
        {
            Register = AddOrReplace;

            // Create Registry and set Factory strategy
            _metadata = new Registry<Type, int[]>();
            _registry = new Registry<NamedType, IPolicySet>(new DefaultPolicies(OptimizingFactory));

            // Register Container as IUnityContainer & IUnityContainerAsync
            var container = new ExplicitRegistration(typeof(UnityContainer), _containerManager)
            {
                BuildChain = new[] { new LifetimeStrategy() }
            };

            // Built-In Features
            _registry.Set(typeof(IUnityContainer), null, container);
            _registry.Set(typeof(IUnityContainerAsync), null, container);

            // Func<> Factory
            var funcBuiltInFctory = new ImplicitRegistration();
            funcBuiltInFctory.Set(typeof(LifetimeManager), new PerResolveLifetimeManager());
            funcBuiltInFctory.Set(typeof(ResolveDelegateFactory), FuncResolver.Factory);
            _registry.Set(typeof(Func<>), funcBuiltInFctory);

            // Lazy<>
            _registry.Set(typeof(Lazy<>), new ImplicitRegistration(typeof(ResolveDelegateFactory), LazyResolver.Factory));

            // Enumerable
            _registry.Set(typeof(IEnumerable<>), new ImplicitRegistration(typeof(ResolveDelegateFactory), EnumerableResolver.Factory));
        }

        internal Action<UnityContainer> SetDefaultPolicies = (UnityContainer container) =>
        {
            // Processors
            var fieldsProcessor = new FieldProcessor(container.Defaults);
            var methodsProcessor = new MethodProcessor(container.Defaults, container);
            var propertiesProcessor = new PropertyProcessor(container.Defaults);
            var constructorProcessor = new ConstructorProcessor(container.Defaults, container);

            // Processors chain
            container._processors = new StagedStrategyChain<MemberProcessor, BuilderStage>
            {
                { constructorProcessor, BuilderStage.Creation },
                { fieldsProcessor,      BuilderStage.Fields },
                { propertiesProcessor,  BuilderStage.Properties },
                { methodsProcessor,     BuilderStage.Methods }
            };

            // Caches
            container._processors.Invalidated += (s, e) => container._processorsChain = container._processors.ToArray();
            container._processorsChain = container._processors.ToArray();

            container.Defaults.CtorSelector = constructorProcessor;
            container.Defaults.PropertiesSelector = propertiesProcessor;
            container.Defaults.MethodsSelector = methodsProcessor;
            container.Defaults.FieldsSelector = fieldsProcessor;
        };

        internal static void SetDiagnosticPolicies(UnityContainer container)
        {
            // Default policies
            container.ContextExecutePlan = UnityContainer.ContextValidatingExecutePlan;
            container.ContextResolvePlan = UnityContainer.ContextValidatingResolvePlan;
            container.ExecutePlan = container.ExecuteValidatingPlan;

            // Processors
            var fieldsProcessor = new FieldDiagnostic(container.Defaults);
            var methodsProcessor = new MethodDiagnostic(container.Defaults, container);
            var propertiesProcessor = new PropertyDiagnostic(container.Defaults);
            var constructorProcessor = new ConstructorDiagnostic(container.Defaults, container);

            // Processors chain
            container._processors = new StagedStrategyChain<MemberProcessor, BuilderStage>
            {
                { constructorProcessor, BuilderStage.Creation },
                { fieldsProcessor,      BuilderStage.Fields },
                { propertiesProcessor,  BuilderStage.Properties },
                { methodsProcessor,     BuilderStage.Methods }
            };

            // Caches
            container._processors.Invalidated += (s, e) => container._processorsChain = container._processors.ToArray();
            container._processorsChain = container._processors.ToArray();

            container.Defaults.ResolveDelegateFactory     = container._buildStrategy;
            container.Defaults.FieldsSelector      = fieldsProcessor;
            container.Defaults.MethodsSelector     = methodsProcessor;
            container.Defaults.PropertiesSelector  = propertiesProcessor;
            container.Defaults.CtorSelector = constructorProcessor;

            var validators = new ImplicitRegistration();

            validators.Set(typeof(Func<Type, InjectionMember, ConstructorInfo>), Validating.ConstructorSelector);
            validators.Set(typeof(Func<Type, InjectionMember, MethodInfo>),      Validating.MethodSelector);
            validators.Set(typeof(Func<Type, InjectionMember, FieldInfo>),       Validating.FieldSelector);
            validators.Set(typeof(Func<Type, InjectionMember, PropertyInfo>),    Validating.PropertySelector);

            container._validators = validators;

            // Registration Validator
            container.TypeValidator = (typeFrom, typeTo) =>
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeFrom != null && !typeFrom.GetTypeInfo().IsGenericType && !typeTo.GetTypeInfo().IsGenericType && 
                                    !typeFrom.GetTypeInfo().IsAssignableFrom(typeTo.GetTypeInfo()))
#else
                if (typeFrom != null && !typeFrom.IsGenericType && !typeTo.IsGenericType &&
                    !typeFrom.IsAssignableFrom(typeTo))
#endif
                {
                    throw new ArgumentException($"The type {typeTo} cannot be assigned to variables of type {typeFrom}.");
                }
            };
        }

        #endregion


        #region Implementation

        private void OnStrategiesChanged(object sender, EventArgs e)
        {
            _strategiesChain = _strategies.ToArray();
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
        private struct ContainerRegistrationStruct : IContainerRegistration
        {
            public Type RegisteredType { get; internal set; }

            public string Name { get; internal set; }

            public Type MappedToType { get; internal set; }

            public LifetimeManager LifetimeManager { get; internal set; }
        }

        #endregion
    }
}
