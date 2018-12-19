using System;
using System.Diagnostics;
using System.Reflection;
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
    public class BuilderContext : IResolveContext
    {
        #region Fields

        private readonly ResolverOverride[] _resolverOverrides;

        #endregion

        #region Constructors

        public BuilderContext(UnityContainer container, InternalRegistration registration,
                              object existing, params ResolverOverride[] resolverOverrides)
        {
            Existing = existing;
            Lifetime = container._lifetimeContainer; 
            Policies = new PolicyList(this);
            Registration = registration;
            Type = registration.Type;
            Name = registration.Name;

            if (null != resolverOverrides && 0 < resolverOverrides.Length)
                _resolverOverrides = resolverOverrides;
        }

        public BuilderContext(BuilderContext original, object existing)
        {
            Existing = existing;
            Lifetime = original.Lifetime;
            Policies = original.Policies;
            Registration = original.Registration;
            Type = original.Type;
            Name = original.Name;
            ParentContext = original;
        }

        internal BuilderContext(ref BuilderContext original, InternalRegistration registration)
        {
            Existing = null;
            Lifetime = original.Lifetime;
            Policies = original.Policies;
            Registration = registration;
            Type = OriginalBuildKey.Type;
            Name = OriginalBuildKey.Name;
            ParentContext = original;

            _resolverOverrides = original._resolverOverrides;
        }

        internal BuilderContext(BuilderContext original, InternalRegistration registration)
        {
            Existing = null;
            Lifetime = original.Lifetime;
            Policies = original.Policies;
            Registration = registration;
            Type = OriginalBuildKey.Type;
            Name = OriginalBuildKey.Name;
            ParentContext = original;

            _resolverOverrides = original._resolverOverrides;
        }

        internal BuilderContext(BuilderContext original, Type type, string name)
        {
            Existing = null;
            Lifetime = original.Lifetime;
            Policies = original.Policies;
            Registration = (InternalRegistration)((UnityContainer)original.Container).GetRegistration(type, name);
            ParentContext = original;
            Type = OriginalBuildKey.Type;
            Name = OriginalBuildKey.Name;

            _resolverOverrides = original._resolverOverrides;
        }

        #endregion


        #region INamedType

        public Type Type { get; set; }

        public string Name { get; }

        #endregion


        #region IResolveContext

        public IUnityContainer Container => Lifetime.Container;

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


        public ILifetimeContainer Lifetime { get; }

        public INamedType OriginalBuildKey => (INamedType) Registration;

        public IPolicySet Registration { get; }

        public IPolicyList Policies { get; }

        public SynchronizedLifetimeManager RequiresRecovery { get; set; }

        public bool BuildComplete { get; set; }

        public BuilderContext ChildContext { get; internal set; }

        public BuilderContext ParentContext { get; }

        #endregion


        #region  : Policies

        object IPolicyList.Get(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                return ((UnityContainer)Container).GetPolicy(type, name, policyInterface);

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
                ((UnityContainer)Container).ClearPolicy(type, name, policyInterface);
            else
                Registration.Clear(policyInterface);
        }

        #endregion
    }
}
