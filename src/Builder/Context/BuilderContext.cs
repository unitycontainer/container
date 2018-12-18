using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Container;
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
            BuildKey = OriginalBuildKey;
            Policies = new PolicyList(this);

            if (null != resolverOverrides && 0 < resolverOverrides.Length)
                _resolverOverrides = resolverOverrides;
        }

        public BuilderContext(IBuilderContext original, IEnumerable<BuilderStrategy> chain, object existing)
        {
            _container = ((BuilderContext)original)._container;
            _chain = new StrategyChain(chain);
            ParentContext = original;
            BuildKey = original.BuildKey;
            Registration = original.Registration;
            Policies = original.Policies;
            Existing = existing;
        }

        internal BuilderContext(ref IBuilderContext original, InternalRegistration registration)
        {
            var parent = (BuilderContext)original;

            _container = parent._container;
            _chain = parent._chain;
            _resolverOverrides = parent._resolverOverrides;
            ParentContext = original;
            Existing = null;
            Policies = parent.Policies;
            Registration = registration;
            BuildKey = OriginalBuildKey;
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
            BuildKey = OriginalBuildKey;
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
            BuildKey = OriginalBuildKey;
        }

        #endregion


        #region INamedType

        public Type Type => BuildKey.Type;

        public string Name => BuildKey.Name;

        #endregion


        #region IResolveContext

        public IUnityContainer Container => _container;

        public object Existing { get; set; }

        public object Resolve(Type type, string name)
        {
            var context = new BuilderContext(this, type, name);
            ChildContext = context;

            var i = -1;
            var chain = ((InternalRegistration)context.Registration).BuildChain;
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
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            var result = context.Existing;

            ChildContext = null;

            return result;
        }

        public object Resolve(PropertyInfo property, string name, object value)
        {
            var context = this;

            // Process overrides if any
            if (null != _resolverOverrides)
            {
                var backup = CurrentOperation;
                CurrentOperation = property;
                // Check for property overrides
                for (var index = _resolverOverrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = _resolverOverrides[index];
                    
                    // Check if this parameter is overridden
                    if (resolverOverride is IEquatable<PropertyInfo> comparer && comparer.Equals(property))
                    {
                        // Check if itself is a value 
                        if (resolverOverride is IResolve resolverPolicy)
                        {
                            return resolverPolicy.Resolve(ref context);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(property.PropertyType);
                        if (null != resolveDelegate)
                        {
                            return resolveDelegate(ref context);
                        }
                    }
                }

                CurrentOperation = backup;
            }

            // Resolve from injectors
            switch (value)
            {
                case PropertyInfo info when ReferenceEquals(info, property):
                    return Resolve(property.PropertyType, name);

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);

                case IResolve policy:
                    return policy.Resolve(ref context);

                case IResolverFactory factory:
                    var method = factory.GetResolver<BuilderContext>(Type);
                    return method?.Invoke(ref context);

                case object obj:
                    return obj;
            }

            // Resolve from container
            return Resolve(property.PropertyType, name);
        }

        public object Resolve(ParameterInfo parameter, string name, object value)
        {
            var context = this;

            // Process overrides if any
            if (null != _resolverOverrides)
            {
                var backup = CurrentOperation;
                CurrentOperation = parameter;

                // Check if this parameter is overridden
                for (var index = _resolverOverrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = _resolverOverrides[index];
                    
                    // If matches with current parameter
                    if (resolverOverride is IEquatable<ParameterInfo> comparer && comparer.Equals(parameter))
                    {
                        // Check if itself is a value 
                        if (resolverOverride is IResolve resolverPolicy)
                        {
                            return resolverPolicy.Resolve(ref context);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(parameter.ParameterType);
                        if (null != resolveDelegate)
                        {
                            return resolveDelegate(ref context);
                        }
                    }
                }

                CurrentOperation = backup;
            }

            // Resolve from injectors
            // TODO: Optimize via overrides
            switch (value)
            {
                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);

                case IResolve policy:
                    return policy.Resolve(ref context);

                case IResolverFactory factory:
                    var method = factory.GetResolver<BuilderContext>(Type);
                    return method?.Invoke(ref context);

                case Type type:     // TODO: Requires evaluation
                    if (typeof(Type) == parameter.ParameterType) return type;
                    break;
                    
                case object obj:
                    return obj;
            }

            // Resolve from container
            return Resolve(parameter.ParameterType, name);
        }

        #endregion


        #region IBuilderContext

        public INamedType BuildKey { get; set; }

        public ILifetimeContainer Lifetime => _container._lifetimeContainer;

        public INamedType OriginalBuildKey => (INamedType) Registration;

        public IPolicySet Registration { get; }

        public IPolicyList Policies { get; }

        public SynchronizedLifetimeManager RequiresRecovery { get; set; }

        public bool BuildComplete { get; set; }

        // TODO: Remove CurrentOperation
        public object CurrentOperation { get; set; }

        public IBuilderContext ChildContext { get; internal set; }

        public IBuilderContext ParentContext { get; }

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
    }
}
