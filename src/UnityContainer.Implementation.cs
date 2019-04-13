using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Events;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Processors;
using Unity.Registration;
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

        // Methods
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Func<Type, string, IPolicySet> _get;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Func<Type, string, Type, IPolicySet> _getGenericRegistration;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal Func<Type, bool> IsTypeExplicitlyRegistered;

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

            // Methods
            _get = _parent._get;
            _getGenericRegistration = _parent._getGenericRegistration;

            // Strategies
            _strategies = _parent._strategies;
            _strategiesChain = _parent._strategiesChain;
            _strategies.Invalidated += OnStrategiesChanged;

            // Caches
            SetDefaultPolicies(this);
        }

        #endregion


        #region Default Policies

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

            container.Defaults.Set(typeof(BuilderContext.ExecutePlanDelegate), container.ContextExecutePlan);
            container.Defaults.Set(typeof(ResolveDelegateFactory), (ResolveDelegateFactory)OptimizingFactory);
            container.Defaults.Set(typeof(ISelect<ConstructorInfo>), constructorProcessor);
            container.Defaults.Set(typeof(ISelect<FieldInfo>), fieldsProcessor);
            container.Defaults.Set(typeof(ISelect<PropertyInfo>), propertiesProcessor);
            container.Defaults.Set(typeof(ISelect<MethodInfo>), methodsProcessor);
        };

        internal static void SetDiagnosticPolicies(UnityContainer container)
        {
            // Default policies
            container.ContextExecutePlan = UnityContainer.ContextValidatingExecutePlan;
            container.ContextResolvePlan = UnityContainer.ContextValidatingResolvePlan;
            container.ExecutePlan = container.ExecuteValidatingPlan;


            if (null != container._registrations) container.Set(null, null, container.Defaults);

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

            container.Defaults.Set(typeof(BuilderContext.ExecutePlanDelegate), container.ContextExecutePlan);
            container.Defaults.Set(typeof(ResolveDelegateFactory), container._buildStrategy);
            container.Defaults.Set(typeof(ISelect<ConstructorInfo>), constructorProcessor);
            container.Defaults.Set(typeof(ISelect<FieldInfo>), fieldsProcessor);
            container.Defaults.Set(typeof(ISelect<PropertyInfo>), propertiesProcessor);
            container.Defaults.Set(typeof(ISelect<MethodInfo>), methodsProcessor);

            var validators = new InternalRegistration();

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

        private void SetupChildContainerBehaviors()
        {
            lock (_syncRoot)
            {
                if (null == _registrations)
                {
                    _registrations = new Registrations(ContainerInitialCapacity);
                    Set(null, null, Defaults);

                    _get = (type, name) => Get(type, name) ?? _parent._get(type, name);
                    _getGenericRegistration = GetOrAddGeneric;
                    IsTypeExplicitlyRegistered = IsTypeTypeExplicitlyRegisteredLocally;
                }
            }

        }

        private void OnStrategiesChanged(object sender, EventArgs e)
        {
            _strategiesChain = _strategies.ToArray();

            if (null != _parent && null == _registrations)
            {
                SetupChildContainerBehaviors();
            }
        }

        private BuilderStrategy[] GetBuilders(Type type, InternalRegistration registration)
        {
            return _strategiesChain.ToArray()
                              .Where(strategy => strategy.RequiredToBuildType(this, type, registration, null))
                              .ToArray();
        }

        internal Type GetFinalType(Type argType)
        {
            Type next;
            for (var type = argType; null != type; type = next)
            {
                var info = type.GetTypeInfo();
                if (info.IsGenericType)
                {
                    if (IsTypeExplicitlyRegistered(type)) return type;

                    var definition = info.GetGenericTypeDefinition();
                    if (IsTypeExplicitlyRegistered(definition)) return definition;

                    next = info.GenericTypeArguments[0];
                    if (IsTypeExplicitlyRegistered(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType();
                    if (IsTypeExplicitlyRegistered(next)) return next;
                }
                else
                {
                    return type;
                }
            }

            return argType;
        }

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// This class doesn't have a finalizer, so <paramref name="disposing"/> will always be true.</remarks>
        /// <param name="disposing">True if being called typeFrom the IDisposable.Dispose
        /// method, false if being called typeFrom a finalizer.</param>
        [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            List<Exception> exceptions = null;

            try
            {
                _strategies.Invalidated -= OnStrategiesChanged;
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
                                                              .ToList())
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

            lock (LifetimeContainer)
            {
                _registrations = new Registrations(1);
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


        #region Nested Types

        private class RegistrationContext : IPolicyList
        {
            private readonly InternalRegistration _registration;
            private readonly UnityContainer _container;
            private readonly Type _type;
            private readonly string _name;

            internal RegistrationContext(UnityContainer container, Type type, string name, InternalRegistration registration)
            {
                _registration = registration;
                _container = container;
                _type = type;
                _name = name;
            }


            #region IPolicyList

            public object Get(Type type, string name, Type policyInterface)
            {
                if (_type != type || _name != name)
                    return _container.GetPolicy(type, name, policyInterface);

                return _registration.Get(policyInterface);
            }


            public void Set(Type type, string name, Type policyInterface, object policy)
            {
                if (_type != type || _name != name)
                    _container.SetPolicy(type, name, policyInterface, policy);
                else
                    _registration.Set(policyInterface, policy);
            }

            public void Clear(Type type, string name, Type policyInterface)
            {
                if (_type != type || _name != name)
                    _container.ClearPolicy(type, name, policyInterface);
                else
                    _registration.Clear(policyInterface);
            }

            #endregion
        }

        #endregion
    }
}
