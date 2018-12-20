using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
#if !NETSTANDARD1_0 && !NETCOREAPP1_0 && !NETCOREAPP2_0
#endif
    [SecuritySafeCritical]
    [DebuggerDisplay("Resolving: {Registration.Type},  Name: {Registration.Name}")]
    public struct BuilderContext : IResolveContext
    {
        #region Fields

        public ResolverOverride[] ResolverOverrides;
        public IPolicyList list;

        #endregion


        #region INamedType

        public Type Type { get; set; }

        public string Name => Registration.Name;

        #endregion


        #region IResolveContext

        public IUnityContainer Container => Lifetime.Container;

        public object Existing { get; set; }

        public object Resolve(Type type, string name) => Resolve((InternalRegistration)
                ((UnityContainer)Container).GetRegistration(type, name));

        public object Resolve(InternalRegistration registration)
        {
            var context = new BuilderContext
            {
                Lifetime = Lifetime,
                Registration = registration,
                Type = registration is ContainerRegistration containerRegistration ? containerRegistration.MappedToType : registration.Type,

                list = list,
                ResolverOverrides = ResolverOverrides,
                Parent = Registration
            };

            return registration.BuildChain.ExecuteReThrowingPlan(ref context);
        }

        public object Resolve(FieldInfo field, string name, object value)
        {
            var context = this;

            // Process overrides if any
            if (null != ResolverOverrides)
            {
                // Check for property overrides
                for (var index = ResolverOverrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = ResolverOverrides[index];

                    // Check if this parameter is overridden
                    if (resolverOverride is IEquatable<FieldInfo> comparer && comparer.Equals(field))
                    {
                        // Check if itself is a value 
                        if (resolverOverride is IResolve resolverPolicy)
                        {
                            return resolverPolicy.Resolve(ref context);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(field.FieldType);
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
                case FieldInfo info when ReferenceEquals(info, field):
                    return Resolve(field.FieldType, name);

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
            return Resolve(field.FieldType, name);
        }

        public object Resolve(PropertyInfo property, string name, object value)
        {
            var context = this;

            // Process overrides if any
            if (null != ResolverOverrides)
            {
                // Check for property overrides
                for (var index = ResolverOverrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = ResolverOverrides[index];

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
            if (null != ResolverOverrides)
            {
                // Check if this parameter is overridden
                for (var index = ResolverOverrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = ResolverOverrides[index];

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


        #region Public Members

        public ILifetimeContainer Lifetime;

        public INamedType Registration;

        public SynchronizedLifetimeManager RequiresRecovery;

        public bool BuildComplete;

        public INamedType Parent { get; private set; } 

        #endregion


        #region  Policies

        public object Get(Type type, string name, Type policyInterface)
        {
            return list.Get(type, name, policyInterface) ??
                   (type != Registration.Type || name != Registration.Name
                       ? ((UnityContainer)Container).GetPolicy(type, name, policyInterface)
                       : ((IPolicySet)Registration).Get(policyInterface));
        }

        public void Set(Type type, string name, Type policyInterface, object policy)
        {
            list.Set(type, name, policyInterface, policy);
        }

        public void Clear(Type type, string name, Type policyInterface)
        {
            list.Clear(type, name, policyInterface);
        }

        #endregion
    }
}
