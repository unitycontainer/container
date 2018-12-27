using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.Events;
using Unity.Extension;
using Unity.Policy;
using Unity.Policy.BuildPlanCreator;
using Unity.Processors;
using Unity.Registration;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    [CLSCompliant(true)]
    public partial class UnityContainer
    {
        #region Delegates

        internal delegate object GetPolicyDelegate(Type type, string name, Type policyInterface);
        internal delegate void SetPolicyDelegate(Type type, string name, Type policyInterface, object policy);
        internal delegate void ClearPolicyDelegate(Type type, string name, Type policyInterface);

        #endregion


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
        private StagedStrategyChain<MemberBuildProcessor, BuilderStage> _buildPlanStrategies;

        // Registrations
        private readonly object _syncRoot = new object();
        private HashRegistry<Type, IRegistry<string, IPolicySet>> _registrations;

        // Events
        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        // Caches
        private BuilderStrategy[]      _strategiesChain;
        private MemberBuildProcessor[] _processorsChain;

        // Methods
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal Func<Type, string, IPolicySet> GetRegistration;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal Func<Type, string, InternalRegistration, IPolicySet> Register;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal GetPolicyDelegate GetPolicy;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal SetPolicyDelegate SetPolicy;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal ClearPolicyDelegate ClearPolicy;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Func<Type, string, IPolicySet> _get;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Func<Type, string, Type, IPolicySet> _getGenericRegistration;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Func<Type, bool> _isTypeExplicitlyRegistered;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Func<Type, string, bool> _isExplicitlyRegistered;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
        {
            _root = this;

            // Lifetime
            LifetimeContainer = new LifetimeContainer(this);

            // Registrations
            _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(ContainerInitialCapacity);

            // Context
            _context = new ContainerContext(this);

            // Methods
            _get = Get;
            _getGenericRegistration = GetOrAddGeneric;
            _isExplicitlyRegistered = IsExplicitlyRegisteredLocally;
            _isTypeExplicitlyRegistered = IsTypeTypeExplicitlyRegisteredLocally;

            GetRegistration = GetOrAdd;
            Register = AddOrUpdate;
            GetPolicy = Get;
            SetPolicy = Set;
            ClearPolicy = Clear;

            // Default Policies and Strategies
            Set(null, null, InitializeDefaultPolicies());
            Set(typeof(Func<>), string.Empty, typeof(LifetimeManager), new PerResolveLifetimeManager());
            Set(typeof(Func<>), string.Empty, typeof(IBuildPlanPolicy), new DeferredResolveCreatorPolicy());
            Set(typeof(Lazy<>), string.Empty, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GenericLazyBuildPlanCreatorPolicy.GetResolver);

            // Register this instance
            ((IUnityContainer)this).RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            // Lifetime
            LifetimeContainer = new LifetimeContainer(this);

            // Context and policies
            _context = new ContainerContext(this);

            // Parent
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _parent.LifetimeContainer.Add(this);
            _root = _parent._root;

            // Methods
            _get = _parent._get;
            _getGenericRegistration = _parent._getGenericRegistration;
            _isExplicitlyRegistered = _parent._isExplicitlyRegistered;
            _isTypeExplicitlyRegistered = _parent._isTypeExplicitlyRegistered;

            GetRegistration = _parent.GetRegistration;
            Register = CreateAndSetOrUpdate;
            GetPolicy = parent.GetPolicy;
            SetPolicy = CreateAndSetPolicy;
            ClearPolicy = delegate { };

            // Strategies
            _strategies = _parent._strategies;
            _buildPlanStrategies = _parent._buildPlanStrategies;
            _strategiesChain = _parent._strategiesChain;
            _processorsChain = _parent._processorsChain;

            // Caches
            _strategies.Invalidated += OnStrategiesChanged;
        }

        #endregion


        #region Defaults

        private IPolicySet InitializeDefaultPolicies()
        {
            // Build Strategies
            _strategies = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>
            {
                {   // Array
                    new ArrayResolveStrategy(
                        typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveArray)),
                        typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveGenericArray))),
                    UnityBuildStage.Enumerable
                },
                {   // Enumerable
                    new EnumerableResolveStrategy(
                        typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveEnumerable)),
                        typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveGenericEnumerable))),
                    UnityBuildStage.Enumerable
                },
                {new BuildKeyMappingStrategy(), UnityBuildStage.TypeMapping},   // Mapping
                {new LifetimeStrategy(), UnityBuildStage.Lifetime},             // Lifetime
                {new BuildPlanStrategy(), UnityBuildStage.Creation}             // Build
            };
            
            // Update on change
            _strategies.Invalidated += OnStrategiesChanged;

            
            // Processors
            var fieldsProcessor = new FieldsProcessor();
            var methodsProcessor = new MethodsProcessor();
            var propertiesProcessor = new PropertiesProcessor();
            var constructorProcessor = new ConstructorProcessor();

            // Processors chain
            _buildPlanStrategies = new StagedStrategyChain<MemberBuildProcessor, BuilderStage>
            {
                { constructorProcessor, BuilderStage.Creation },
                { fieldsProcessor,      BuilderStage.Fields },
                { propertiesProcessor,  BuilderStage.Properties },
                { methodsProcessor,     BuilderStage.Methods }
            };

            // Update on change
            _buildPlanStrategies.Invalidated += (s, e) => _processorsChain = _buildPlanStrategies.ToArray();


            // Caches
            _strategiesChain = _strategies.ToArray();
            _processorsChain = _buildPlanStrategies.ToArray();


            // Default policies
            var defaults = new InternalRegistration(null, null);

            defaults.Set(typeof(ResolveDelegateFactory),   (ResolveDelegateFactory)GetResolver);
            defaults.Set(typeof(ISelect<ConstructorInfo>), constructorProcessor);
            defaults.Set(typeof(ISelect<FieldInfo>),       fieldsProcessor);
            defaults.Set(typeof(ISelect<PropertyInfo>),    propertiesProcessor);
            defaults.Set(typeof(ISelect<MethodInfo>),      methodsProcessor);

            return defaults;
        }

        #endregion


        #region Implementation

        private void CreateAndSetPolicy(Type type, string name, Type policyInterface, object policy)
        {
            lock (GetRegistration)
            {
                if (null == _registrations)
                    SetupChildContainerBehaviors();
            }

            Set(type, name, policyInterface, policy);
        }

        private IPolicySet CreateAndSetOrUpdate(Type type, string name, InternalRegistration registration)
        {
            lock (GetRegistration)
            {
                if (null == _registrations)
                    SetupChildContainerBehaviors();
            }

            return AddOrUpdate(type, name, registration);
        }

        private void SetupChildContainerBehaviors()
        {
            _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(ContainerInitialCapacity);

            Register = AddOrUpdate;
            GetPolicy = Get;
            SetPolicy = Set;
            ClearPolicy = Clear;

            GetRegistration = GetDynamicRegistration;

            _get = (type, name) => Get(type, name) ?? _parent._get(type, name);
            _getGenericRegistration = GetOrAddGeneric;
            _isTypeExplicitlyRegistered = IsTypeTypeExplicitlyRegisteredLocally;
            _isExplicitlyRegistered = IsExplicitlyRegisteredLocally;
        }

        private void OnStrategiesChanged(object sender, EventArgs e)
        {
            _strategiesChain = _strategies.ToArray();

            if (null != _parent && null == _registrations)
            {
                SetupChildContainerBehaviors();
            }
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
                    if (_isTypeExplicitlyRegistered(type)) return type;

                    var definition = info.GetGenericTypeDefinition();
                    if (_isTypeExplicitlyRegistered(definition)) return definition;

                    next = info.GenericTypeArguments[0];
                    if (_isTypeExplicitlyRegistered(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType();
                    if (_isTypeExplicitlyRegistered(next)) return next;
                }
                else
                {
                    return type;
                }
            }

            return argType;
        }

        private static object ExecutePlan(BuilderStrategy[] chain, ref BuilderContext context)
        {
            var i = -1;

            try
            {
                while (!context.BuildComplete && ++i < chain.Length)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception ex)
            {
                context.RequiresRecovery?.Recover();
                // TODO: 5.9.0 Add proper error message
                throw new ResolutionFailedException(context.RegistrationType,
                    context.RegistrationName,
                    "", ex);
            }

            return context.Existing;
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

            lock (GetRegistration)
            {
                _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(1);
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
