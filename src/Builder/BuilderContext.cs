using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {OriginalBuildKey.Type},  Name: {OriginalBuildKey.Name}")]
    public class BuilderContext : IBuilderContext
    {
        #region Fields

        private readonly IStrategyChain _chain;
        private readonly ResolverOverride[] _resolverOverrides;
        readonly UnityContainer _container;

        #endregion

        #region Constructors

        public BuilderContext(UnityContainer container, InternalRegistration registration,
                              object existing, params ResolverOverride[] resolverOverrides)
        {
            _container = container;
            _chain = _container._strategyChain;

            Existing = existing;
            Registration = registration;
            OriginalBuildKey = registration;
            BuildKey = OriginalBuildKey;
            TypeInfo = BuildKey.Type.GetTypeInfo();
            Policies = new PolicyList(this);

            if (null != resolverOverrides && 0 < resolverOverrides.Length)
                _resolverOverrides = resolverOverrides;
        }

        public BuilderContext(IBuilderContext original, IEnumerable<BuilderStrategy> chain, object existing)
        {
            _container = ((BuilderContext)original)._container;
            _chain = new StrategyChain(chain);
            ParentContext = original;
            OriginalBuildKey = original.OriginalBuildKey;
            BuildKey = original.BuildKey;
            TypeInfo = BuildKey.Type.GetTypeInfo();
            Registration = original.Registration;
            Policies = original.Policies;
            Existing = existing;
        }

        internal BuilderContext(IBuilderContext original, InternalRegistration registration)
        {
            var parent = (BuilderContext)original;

            _container = parent._container;
            _chain = parent._chain;
            _resolverOverrides = parent._resolverOverrides;
            ParentContext = original;
            Existing = null;
            Policies = parent.Policies;
            Registration = registration;
            OriginalBuildKey = (INamedType)Registration;
            BuildKey = OriginalBuildKey;
            TypeInfo = BuildKey.Type.GetTypeInfo();
        }

        internal BuilderContext(IBuilderContext original, Type type, string name)
        {
            var parent = (BuilderContext)original;
            var registration = (InternalRegistration)parent._container.GetRegistration(type, name);

            _container = parent._container;
            _chain = parent._chain;
            _resolverOverrides = parent._resolverOverrides;
            ParentContext = original;
            Existing = null;
            Policies = parent.Policies;
            Registration = registration;
            OriginalBuildKey = registration;
            BuildKey = OriginalBuildKey;
            TypeInfo = BuildKey.Type.GetTypeInfo();
        }

        #endregion


        #region INamedType

        public Type Type => BuildKey.Type;

        public string Name => BuildKey.Name;

        #endregion


        #region IBuildContext

        public IUnityContainer Container => _container;

        public object Existing { get; set; }

        public object Resolve(Type type, string name) => NewBuildUp(type, name);

        public object Resolve(PropertyInfo property, string name, IResolverPolicy resolver = null)
        {
            var context = this;

            if (null != _resolverOverrides)
            {
                var backup = CurrentOperation;
                CurrentOperation = property;

                for (var index = _resolverOverrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = _resolverOverrides[index];

                    if (resolverOverride is IEquatable<PropertyInfo> comparer && comparer.Equals(property))
                    {
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(property.PropertyType);
                        if (null != resolveDelegate)
                        {
                            return resolveDelegate(ref context);
                        }
                    }
                }

                CurrentOperation = backup;
            }

            return null != resolver 
                ? resolver.Resolve(ref context) 
                : Resolve(property.PropertyType, name);
        }

        public object Resolve(ParameterInfo parameter, string name, IResolverPolicy resolver = null)
        {
            var context = this;

            if (null != _resolverOverrides)
            {
                var backup = CurrentOperation;
                CurrentOperation = parameter;
                for (var index = _resolverOverrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = _resolverOverrides[index];

                    if (resolverOverride is IEquatable<ParameterInfo> comparer && comparer.Equals(parameter))
                    {
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(parameter.ParameterType);
                        if (null != resolveDelegate)
                        {
                            return resolveDelegate(ref context);
                        }
                    }
                }

                CurrentOperation = backup;
            }

            return null != resolver
                ? resolver.Resolve(ref context)
                : Resolve(parameter.ParameterType, name);
        }

        #endregion


        #region IBuilderContext

        public TypeInfo TypeInfo { get; }

        public IStrategyChain Strategies => _chain;

        public INamedType BuildKey { get; set; }

        public ILifetimeContainer Lifetime => _container._lifetimeContainer;

        public INamedType OriginalBuildKey { get; }

        public IPolicySet Registration { get; }

        public IPolicyList Policies { get; }

        public IRequiresRecovery RequiresRecovery { get; set; }

        public bool BuildComplete { get; set; }

        public object CurrentOperation { get; set; }

        public IBuilderContext ChildContext { get; internal set; }

        public IBuilderContext ParentContext { get; }

        public IPolicyList PersistentPolicies => this;

        #endregion

        
        #region Build

        public object BuildUp()
        {
            var i = -1;
            var chain = ((InternalRegistration)Registration).BuildChain;
            var context = this;
            try
            {
                while (!BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception)
            {
                RequiresRecovery?.Recover();
                throw;
            }

            return Existing;
        }

        public object NewBuildUp(INamedType namedType)
        {
            InternalRegistration registration = (InternalRegistration) namedType;
            var context = new BuilderContext(this, registration);
            ChildContext = context;

            var i = -1;
            var chain = registration.BuildChain;
            try
            {
                while (!context.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            var result = context.Existing;
            ChildContext = null;

            return result;
        }

        public object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null)
        {
            var context = new BuilderContext(this, type, name);
            ChildContext = context;
            childCustomizationBlock?.Invoke(ChildContext);

            var i = -1;
            var chain = ((InternalRegistration)context.Registration).BuildChain;
            try
            {
                while (!context.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            var result = context.Existing;
            ChildContext = null;

            return result;
        }

        #endregion


        #region  : Policies

        object IPolicyList.Get(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                return _container.GetPolicy(type, name, policyInterface);

            var result = Registration.Get(policyInterface);

            return result;
        }

        void IPolicyList.Set(Type type, string name, Type policyInterface, object policy)
        {
            Policies.Set(type, name, policyInterface, policy);
        }

        void IPolicyList.Clear(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                _container.ClearPolicy(type, name, policyInterface);
            else
                Registration.Clear(policyInterface);
        }

        #endregion


        #region Registration

        object Get(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                return _container.GetPolicy(type, name, policyInterface);

            return Registration.Get(policyInterface);

        }

        void Set(Type type, string name, Type policyInterface, object policy)
        {
            if (type != OriginalBuildKey.Type || name != OriginalBuildKey.Name)
                _container.SetPolicy(type, name, policyInterface, policy);
            else
                Registration.Set(policyInterface, policy);
        }

        void Clear(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                _container.ClearPolicy(type, name, policyInterface);
            else
                Registration.Clear(policyInterface);
        }

        #endregion
    }
}
