﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [CLSCompliant(true)]
    public partial class UnityContainer
    {
        #region Delegates

        internal delegate IBuilderPolicy GetPolicyDelegate(Type type, string name, Type policyInterface, out IPolicyList list);
        internal delegate void SetPolicyDelegate(Type type, string name, Type policyInterface, IBuilderPolicy policy);
        internal delegate void ClearPolicyDelegate(Type type, string name, Type policyInterface);

        #endregion


        #region Fields

        // Container specific
        private readonly UnityContainer _parent;
        internal readonly LifetimeContainer _lifetimeContainer;
        private List<UnityContainerExtension> _extensions;
        private UnityContainer _root;

        // Policies
        private readonly ContainerContext _context;

        // Strategies
        private StagedStrategyChain<BuilderStrategy, UnityBuildStage> _strategies;
        private StagedStrategyChain<BuilderStrategy, BuilderStage> _buildPlanStrategies;

        // Registrations
        private readonly object _syncRoot = new object();
        private HashRegistry<Type, IRegistry<string, IPolicySet>> _registrations;

        // Events
        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        // Caches
        internal IStrategyChain _strategyChain;
        internal BuilderStrategy[] _buildChain;

        // Methods
        internal Func<Type, string, IPolicySet> GetRegistration;
        internal Func<IBuilderContext, object> BuildUpPipeline;
        internal Func<INamedType, IPolicySet> Register;
        internal GetPolicyDelegate GetPolicy;
        internal SetPolicyDelegate SetPolicy;
        internal ClearPolicyDelegate ClearPolicy;
        internal Func<Type, string, bool> RegistrationExists;

        private Func<Type, string, IPolicySet> _get;
        private Func<Type, string, Type, IPolicySet> _getGenericRegistration;
        private Func<Type, bool> _isTypeExplicitlyRegistered;
        private Func<Type, string, bool> _isExplicitlyRegistered;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
        {
            _root = this;

            // Lifetime
            _lifetimeContainer = new LifetimeContainer(this);

            // Registrations
            _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(ContainerInitialCapacity);

            // Context and policies
            _context = new ContainerContext(this);
            _strategies = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>();
            _buildPlanStrategies = new StagedStrategyChain<BuilderStrategy, BuilderStage>();

            // Methods
            _get = Get;
            _getGenericRegistration = GetOrAddGeneric;
            _isExplicitlyRegistered = IsExplicitlyRegisteredLocally;
            _isTypeExplicitlyRegistered = IsTypeTypeExplicitlyRegisteredLocally;

            BuildUpPipeline = ThrowingBuildUp;
            GetRegistration = GetOrAdd;
            Register = AddOrUpdate;
            GetPolicy = Get;
            SetPolicy = Set;
            ClearPolicy = Clear;
            RegistrationExists = (type, name) => null != _get(type, name);

            // TODO: Initialize disposables 
            _lifetimeContainer.Add(_strategies);
            _lifetimeContainer.Add(_buildPlanStrategies);

            // Main strategy chain
            _strategies.Add(new ArrayResolveStrategy(typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveArray)),
                                                     typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveGenericArray))), UnityBuildStage.Enumerable);
            _strategies.Add(new EnumerableResolveStrategy(typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveEnumerable)),
                                                          typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveGenericEnumerable))), UnityBuildStage.Enumerable);
            _strategies.Add(new BuildKeyMappingStrategy(), UnityBuildStage.TypeMapping);
            _strategies.Add(new LifetimeStrategy(), UnityBuildStage.Lifetime);
            _strategies.Add(new BuildPlanStrategy(), UnityBuildStage.Creation);

            // Build plan strategy chain
            _buildPlanStrategies.Add(new DynamicMethodConstructorStrategy(), BuilderStage.Creation);
            _buildPlanStrategies.Add(new DynamicMethodPropertySetterStrategy(), BuilderStage.Initialization);
            _buildPlanStrategies.Add(new DynamicMethodCallStrategy(), BuilderStage.Initialization);

            // Caches
            _strategyChain = new StrategyChain(_strategies);
            _buildChain = _strategies.ToArray();
            _strategies.Invalidated += OnStrategiesChanged;

            // Default Policies
            Set(null, null, GetDefaultPolicies());
            Set(typeof(Func<>), string.Empty, typeof(ILifetimePolicy), new PerResolveLifetimeManager());
            Set(typeof(Func<>), string.Empty, typeof(IBuildPlanPolicy), new DeferredResolveCreatorPolicy());
            Set(typeof(Lazy<>), string.Empty, typeof(IBuildPlanCreatorPolicy), new GenericLazyBuildPlanCreatorPolicy());

            // Register this instance
            RegisterInstance(typeof(IUnityContainer), null, this, new ContainerLifetimeManager());
        }

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            // Lifetime
            _lifetimeContainer = new LifetimeContainer(this);

            // Context and policies
            _context = new ContainerContext(this);

            // Parent
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _parent._lifetimeContainer.Add(this);
            _root = _parent._root;

            // Methods
            _get = _parent._get;
            _getGenericRegistration = _parent._getGenericRegistration;
            _isExplicitlyRegistered = _parent._isExplicitlyRegistered;
            _isTypeExplicitlyRegistered = _parent._isTypeExplicitlyRegistered;

            BuildUpPipeline = _parent.BuildUpPipeline;
            GetRegistration = _parent.GetRegistration;
            Register = CreateAndSetOrUpdate;
            GetPolicy = parent.GetPolicy;
            SetPolicy = CreateAndSetPolicy;
            ClearPolicy = delegate { };
            RegistrationExists = (type, name) => null != _get(type, name);

            // Strategies
            _strategies = _parent._strategies;
            _buildPlanStrategies = _parent._buildPlanStrategies;
            _strategyChain = _parent._strategyChain;
            _buildChain = _parent._buildChain;

            // Caches
            _strategies.Invalidated += OnStrategiesChanged;
        }

        #endregion


        #region Defaults

        private IPolicySet GetDefaultPolicies()
        {
            var defaults = new InternalRegistration(null, null);

            defaults.Set(typeof(IBuildPlanCreatorPolicy), new DynamicMethodBuildPlanCreatorPolicy(_buildPlanStrategies));
            defaults.Set(typeof(IConstructorSelectorPolicy), new DefaultUnityConstructorSelectorPolicy());
            defaults.Set(typeof(IPropertySelectorPolicy), new DefaultUnityPropertySelectorPolicy());
            defaults.Set(typeof(IMethodSelectorPolicy), new DefaultUnityMethodSelectorPolicy());

            return defaults;
        }

        #endregion


        #region Implementation

        private void CreateAndSetPolicy(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            lock (GetRegistration)
            {
                if (null == _registrations)
                    SetupChildContainerBehaviors();
            }

            Set(type, name, policyInterface, policy);
        }

        private IPolicySet CreateAndSetOrUpdate(INamedType registration)
        {
            lock (GetRegistration)
            {
                if (null == _registrations)
                    SetupChildContainerBehaviors();
            }

            return AddOrUpdate(registration);
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
            _strategyChain = new StrategyChain(_strategies);
            _buildChain = _strategies.ToArray();
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

        private IList<BuilderStrategy> GetBuilders(InternalRegistration registration)
        {
            var chain = new List<BuilderStrategy>();
            var strategies = _buildChain;

            foreach (var strategy in strategies)
            {
                if (registration.Type != null && strategy.RequiredToBuildType(this, registration, null))
                    chain.Add(strategy);
            }

            return chain;
        }



        private static readonly HashSet<Type> _set = new HashSet<Type>
        {
            typeof(Func<>), typeof(Func<,>), typeof(Func<,,>),      // etc
        };

        [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
        internal Type GetFinalType(Type argType)
        {
            Type next;
            for (var type = argType; null != type; type = next)
            {
                var info = type.GetTypeInfo();

                if (type.IsArray)
                {
                    next = type.GetElementType();
                    if (_isTypeExplicitlyRegistered(next)) return next;
                }
                else if (info.IsGenericType)
                {
                    var definition = info.GetGenericTypeDefinition();

                    if (definition == typeof(Lazy<>) ||
                      definition == typeof(IEnumerable<>) ||
                        _set.Contains(info.GetGenericTypeDefinition()))
                    {
                        if (_isTypeExplicitlyRegistered(type)) return type;


                        if (_isTypeExplicitlyRegistered(definition)) return definition;

                        next = info.GenericTypeArguments[0];
                        if (_isTypeExplicitlyRegistered(next)) return next;

                    }
                    else
                    {
                        return definition;
                    }
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
                _parent?._lifetimeContainer.Remove(this);
                _lifetimeContainer.Dispose();
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

            internal RegistrationContext(UnityContainer container, InternalRegistration registration)
            {
                _registration = registration;
                _container = container;
            }


            #region IPolicyList

            public IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
            {
                if (_registration.Type != type || _registration.Name != name)
                    return _container.GetPolicy(type, name, policyInterface, out list);

                list = this;
                return _registration.Get(policyInterface);
            }


            public void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
            {
                if (_registration.Type != type || _registration.Name != name)
                    _container.SetPolicy(type, name, policyInterface, policy);
                else
                    _registration.Set(policyInterface, policy);
            }

            public void Clear(Type type, string name, Type policyInterface)
            {
                if (_registration.Type != type || _registration.Name != name)
                    _container.ClearPolicy(type, name, policyInterface);
                else
                    _registration.Clear(policyInterface);
            }

            public void ClearAll()
            {
            }

            #endregion
        }

        #endregion
    }
}
