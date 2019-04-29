using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using Unity.Builder;
using Unity.Events;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Processors;
using Unity.Registration;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    [CLSCompliant(true)]
    [SecuritySafeCritical]
    public partial class UnityContainer
    {
        #region Fields

        // Container specific
        private readonly UnityContainer _root;
        private readonly UnityContainer? _parent;
        internal readonly LifetimeContainer LifetimeContainer;
        private List<IUnityContainerExtensionConfigurator>? _extensions;

        // Policies
        private readonly ContainerContext _context;

        // Strategies
        private StagedStrategyChain<BuilderStrategy, UnityBuildStage> _strategies;

        // Caches
        private BuilderStrategy[] _strategiesChain;
        private PipelineProcessor[] _processorsChain;

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
            _strategies.Invalidated += (s, e) => _strategiesChain = _strategies.ToArray(); 

            // Caches
            SetDefaultPolicies(this);
        }

        #endregion


        #region Default Policies

        // TODO: Requires refactoring
        internal Action<UnityContainer> SetDefaultPolicies = (UnityContainer container) =>
        {
            // Processors
            var fieldsProcessor = new FieldProcessor(container.Defaults);
            var methodsProcessor = new MethodProcessor(container.Defaults, container);
            var propertiesProcessor = new PropertyProcessor(container.Defaults);
            var constructorProcessor = new ConstructorProcessor(container.Defaults, container);
            var lifetimeProcessor = new LifetimeProcessor();
            var mappingProcessor = new MappingProcessor();

            // Processors for implicit registrations
            container._processors = new StagedStrategyChain<PipelineProcessor, CompositionStage>
            {
                { lifetimeProcessor,    CompositionStage.Lifetime },
                { mappingProcessor,     CompositionStage.TypeMapping },
                { constructorProcessor, CompositionStage.Creation },
                { fieldsProcessor,      CompositionStage.Fields },
                { propertiesProcessor,  CompositionStage.Properties },
                { methodsProcessor,     CompositionStage.Methods }
            };

            // Processors for Factory Registrations
            container._processorsFactory = new StagedStrategyChain<PipelineProcessor, CompositionStage>
            {
                { lifetimeProcessor,      CompositionStage.Lifetime },
                { new FactoryProcessor(), CompositionStage.Creation },
            };

            // Processors for Instance Registrations
            container._processorsInstance = new StagedStrategyChain<PipelineProcessor, CompositionStage>
            {
                { lifetimeProcessor,    CompositionStage.Lifetime },
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
            container.Compose = container.ValidatingComposePlan;
            container.ContextExecutePlan = ContextValidatingExecutePlan;
            container.ContextResolvePlan = ContextValidatingResolvePlan;
            container.ExecutePlan = container.ExecuteValidatingPlan;

            // Processors
            var fieldsProcessor = new FieldDiagnostic(container.Defaults);
            var methodsProcessor = new MethodDiagnostic(container.Defaults, container);
            var propertiesProcessor = new PropertyDiagnostic(container.Defaults);
            var constructorProcessor = new ConstructorDiagnostic(container.Defaults, container);
            var lifetimeProcessor = new LifetimeProcessor();
            var mappingProcessor = new MappingProcessor();

            // Processors for implicit registrations
            container._processors = new StagedStrategyChain<PipelineProcessor, CompositionStage>
            {
                { lifetimeProcessor,    CompositionStage.Lifetime },
                { mappingProcessor,     CompositionStage.TypeMapping },
                { constructorProcessor, CompositionStage.Creation },
                { fieldsProcessor,      CompositionStage.Fields },
                { propertiesProcessor,  CompositionStage.Properties },
                { methodsProcessor,     CompositionStage.Methods }
            };

            // Processors for Factory Registrations
            container._processorsFactory = new StagedStrategyChain<PipelineProcessor, CompositionStage>
            {
                { lifetimeProcessor,      CompositionStage.Lifetime },
                { new FactoryProcessor(), CompositionStage.Creation },
            };

            // Processors for Instance Registrations
            container._processorsInstance = new StagedStrategyChain<PipelineProcessor, CompositionStage>
            {
                { lifetimeProcessor,    CompositionStage.Lifetime },
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


        #region Nested Types

        [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
        private struct ContainerRegistrationStruct : IContainerRegistration
        {
            public Type RegisteredType { get; internal set; }

            public string Name { get; internal set; }

            public Type? MappedToType { get; internal set; }

            public LifetimeManager LifetimeManager { get; internal set; }
        }

        #endregion
    }
}
