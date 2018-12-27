using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [SecuritySafeCritical]
    [DebuggerDisplay("Resolving: {Type},  Name: {Name}")]
    public struct BuilderContext : IResolveContext
    {
        #region Fields

        public ResolverOverride[] Overrides;
        public IPolicyList list;

        #endregion


        #region IResolveContext

        public IUnityContainer Container => Lifetime?.Container;

        public Type Type { get; set; }

        public string Name => RegistrationName;

        public object Resolve(Type type, string name) => Resolve(type, name,
            (InternalRegistration)((UnityContainer)Container).GetRegistration(type, name));

        #endregion


        #region IPolicyList

        public object Get(Type type, string name, Type policyInterface)
        {
            return list.Get(type, name, policyInterface) ??
                   (type != RegistrationType || name != RegistrationName
                       ? ((UnityContainer)Container).GetPolicy(type, name, policyInterface)
                       : Registration.Get(policyInterface));
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


        #region Registration

        public Type RegistrationType { get; set; }

        public string RegistrationName { get; set; }

        public IPolicySet Registration { get; set; }

        #endregion


        #region Public Properties

        public object Existing { get; set; }

        public ILifetimeContainer Lifetime;

        public SynchronizedLifetimeManager RequiresRecovery;

        public bool BuildComplete;

        public Type DeclaringType { get; private set; }

        #endregion


        #region Public Methods

        public object Resolve(Type type, string name, InternalRegistration registration)
        {
            var context = new BuilderContext
            {
                Lifetime = Lifetime,
                Registration = registration,
                RegistrationType = type,
                RegistrationName = name,
                Type = registration is ContainerRegistration containerRegistration ? containerRegistration.Type : type,

                list = list,
                Overrides = Overrides,
                DeclaringType = RegistrationType
            };

            return ExecuteReThrowingPlan(registration.BuildChain, ref context);
        }

        public object Resolve(ParameterInfo parameter, string name, object value)
        {
            var context = this;

            // Process overrides if any
            if (null != Overrides)
            {
                // Check if this parameter is overridden
                for (var index = Overrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = Overrides[index];

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

        public object Resolve(FieldInfo field, string name, object value)
        {
            var context = this;

            // Process overrides if any
            if (null != Overrides)
            {
                // Check for property overrides
                for (var index = Overrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = Overrides[index];

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
            if (null != Overrides)
            {
                // Check for property overrides
                for (var index = Overrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = Overrides[index];

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

        #endregion


        #region Implementation


        private static object ExecuteReThrowingPlan(BuilderStrategy[] chain, ref BuilderContext context)
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
            catch
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            return context.Existing;
        }

        #endregion
    }
}
