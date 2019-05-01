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
using Unity.Pipeline;
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

            // Context and policies
            _context = new ContainerContext(this);

            // Strategies
            _strategies = _parent._strategies;
            _strategiesChain = _parent._strategiesChain;
            _strategies.Invalidated += (s, e) => _strategiesChain = _strategies.ToArray();
        }

        #endregion


        #region Default Policies

        internal void SetDiagnosticPolicies()
        {
            // Default policies
            Compose = ValidatingComposePlan;
            ContextExecutePlan = ContextValidatingExecutePlan;
            ContextResolvePlan = ContextValidatingResolvePlan;
            ExecutePlan = ExecuteValidatingPlan;

            // Builders
            var lifetimeBuilder    = new LifetimeBuilder();
            var mappingBuilder     = new MappingBuilder();
            var factoryBuilder     = new FactoryBuilder();
            var constructorBuilder = new ConstructorDiagnostic(this);
            var fieldsBuilder      = new FieldDiagnostic(this);
            var propertiesBuilder  = new PropertyDiagnostic(this);
            var methodsBuilder     = new MethodDiagnostic(this);

            Defaults.TypeStages = new StagedStrategyChain<PipelineBuilder, PipelineStage>
            {
                { lifetimeBuilder,    PipelineStage.Lifetime },
                { mappingBuilder,     PipelineStage.TypeMapping },
                { factoryBuilder, PipelineStage.Factory },
                { constructorBuilder, PipelineStage.Creation },
                { fieldsBuilder,      PipelineStage.Fields },
                { propertiesBuilder,  PipelineStage.Properties },
                { methodsBuilder,     PipelineStage.Methods }
            };

            Defaults.FactoryStages = new StagedStrategyChain<PipelineBuilder, PipelineStage>
            {
                { lifetimeBuilder, PipelineStage.Lifetime },
                { factoryBuilder,  PipelineStage.Factory }
            };

            Defaults.InstanceStages = new StagedStrategyChain<PipelineBuilder, PipelineStage>
            {
                { lifetimeBuilder, PipelineStage.Lifetime },
                { factoryBuilder,  PipelineStage.Factory },
            };

            // Selectors
            Defaults.CtorSelector = constructorBuilder;
            Defaults.FieldsSelector = fieldsBuilder;
            Defaults.PropertiesSelector = propertiesBuilder;
            Defaults.MethodsSelector = methodsBuilder;

            var validators = new ImplicitRegistration();

            validators.Set(typeof(Func<Type, InjectionMember, ConstructorInfo>), Validating.ConstructorSelector);
            validators.Set(typeof(Func<Type, InjectionMember, MethodInfo>), Validating.MethodSelector);
            validators.Set(typeof(Func<Type, InjectionMember, FieldInfo>), Validating.FieldSelector);
            validators.Set(typeof(Func<Type, InjectionMember, PropertyInfo>), Validating.PropertySelector);

            _validators = validators;

            // Registration Validator
            TypeValidator = (typeFrom, typeTo) =>
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
