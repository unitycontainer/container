using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Exceptions;
using Unity.Factories;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
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
        internal IPolicyList List;

        public delegate object ExecutePlanDelegate(BuilderStrategy[] chain, ref BuilderContext context);
        public delegate object ResolvePlanDelegate(ref BuilderContext context, ResolveDelegate<BuilderContext> resolver);

        #endregion


        #region IResolveContext

        public IUnityContainer Container => Lifetime?.Container;

        public Type Type { get; set; }

        public string Name { get; set; }

        public object Resolve(Type type, string name)
        {
            // Process overrides if any
            if (null != Overrides)
            {
                NamedType namedType = new NamedType
                {
                    Type = type,
                    Name = name
                };

                // Check if this parameter is overridden
                for (var index = Overrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = Overrides[index];
                    // If matches with current parameter
                    if (resolverOverride is IResolve resolverPolicy &&
                        resolverOverride is IEquatable<NamedType> comparer && comparer.Equals(namedType))
                    {
                        var context = this;

                        return ResolvePlan(ref context, resolverPolicy.Resolve);
                    }
                }
            }

            return Resolve(type, name, ((UnityContainer)Container).GetRegistration(type, name));
        }

        #endregion


        #region IPolicyList

        public object Get(Type policyInterface)
        {
            return List.Get(RegistrationType, Name, policyInterface) ??
                   Registration.Get(policyInterface);
        }


        public object Get(Type type, Type policyInterface)
        {
            return List.Get(type, policyInterface) ??
                   (type != RegistrationType
                       ? ((UnityContainer)Container).GetPolicy(type, policyInterface)
                       : Registration.Get(policyInterface));
        }

        public object Get(Type type, string name, Type policyInterface)
        {
            return List.Get(type, name, policyInterface) ??
                   (type != RegistrationType || name != Name
                       ? ((UnityContainer)Container).GetPolicy(type, name, policyInterface)
                       : Registration.Get(policyInterface));
        }

        public void Set(Type policyInterface, object policy)
        {
            List.Set(RegistrationType, Name, policyInterface, policy);
        }


        public void Set(Type type, Type policyInterface, object policy)
        {
            List.Set(type, policyInterface, policy);
        }

        public void Set(Type type, string name, Type policyInterface, object policy)
        {
            List.Set(type, name, policyInterface, policy);
        }

        public void Clear(Type type, string name, Type policyInterface)
        {
            List.Clear(type, name, policyInterface);
        }

        #endregion


        #region Registration

        public Type RegistrationType { get; set; }

        public IPolicySet Registration { get; set; }

        #endregion


        #region Public Properties

        public object Existing { get; set; }

        public ILifetimeContainer Lifetime;

        public SynchronizedLifetimeManager RequiresRecovery;

        public bool BuildComplete;

        public Type DeclaringType;
#if !NET40
        public IntPtr Parent;
#endif
        public ExecutePlanDelegate ExecutePlan;

        public ResolvePlanDelegate ResolvePlan;

        #endregion


        #region Resolve Methods

        public object Resolve(Type type, string name, InternalRegistration registration)
        {
            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    Lifetime = Lifetime,
                    Registration = registration,
                    RegistrationType = type,
                    Name = name,
                    Type = registration is ContainerRegistration containerRegistration ? containerRegistration.Type : type,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                return ExecutePlan(registration.BuildChain, ref context);
            }
        }

        public object Resolve(ParameterInfo parameter, object value)
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
                            return ResolvePlan(ref context, resolverPolicy.Resolve);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(parameter.ParameterType);
                        if (null != resolveDelegate)
                        {
                            return ResolvePlan(ref context, resolveDelegate);
                        }
                    }
                }
            }

            // Resolve from injectors
            switch (value)
            {
                case ParameterInfo info
                when ReferenceEquals(info, parameter):
                    return Resolve(parameter.ParameterType, null);

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);
            }

            return value;
        }

        public object Resolve(PropertyInfo property, object value)
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
                            return ResolvePlan(ref context, resolverPolicy.Resolve);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(property.PropertyType);
                        if (null != resolveDelegate)
                        {
                            return ResolvePlan(ref context, resolveDelegate);
                        }
                    }
                }
            }

            // Resolve from injectors
            switch (value)
            {
                case DependencyAttribute dependencyAttribute:
                    return Resolve(property.PropertyType, dependencyAttribute.Name);

                case OptionalDependencyAttribute optionalAttribute:
                    try
                    {
                        return Resolve(property.PropertyType, optionalAttribute.Name);
                    }
                    catch (Exception ex) 
                    when (!(ex.InnerException is CircularDependencyException))
                    {
                        return null;
                    }

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);
            }

            return value;
        }

        public object Resolve(FieldInfo field, object value)
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
                            return ResolvePlan(ref context, resolverPolicy.Resolve);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(field.FieldType);
                        if (null != resolveDelegate)
                        {
                            return ResolvePlan(ref context, resolveDelegate);
                        }
                    }
                }
            }

            // Resolve from injectors
            switch (value)
            {
                case DependencyAttribute dependencyAttribute:
                    return Resolve(field.FieldType, dependencyAttribute.Name);

                case OptionalDependencyAttribute optionalAttribute:
                    try
                    {
                        return Resolve(field.FieldType, optionalAttribute.Name);
                    }
                    catch (Exception ex) 
                    when (!(ex.InnerException is CircularDependencyException))
                    {
                        return null;
                    }

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);
            }

            return value;
        }

        #endregion


        #region Policy Methods

        public ResolveDelegate<BuilderContext> GetResolver()
        {
            // Get it from the list
            ResolveDelegate<BuilderContext> policy = Registration.Get<ResolveDelegate<BuilderContext>>();
            if (null != policy) return policy;

            if (Registration is ContainerRegistration registration)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (Type?.GetTypeInfo().IsGenericType ?? false)
#else
                if (Type?.IsGenericType ?? false)
#endif
                    policy = ((UnityContainer)Container).GetResolverPolicy(Type.GetGenericTypeDefinition(), Name);
            }
            else
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (RegistrationType.GetTypeInfo().IsGenericType)
#else
                if (RegistrationType.IsGenericType)
#endif
                    policy = ((UnityContainer)Container).GetResolverPolicy(RegistrationType.GetGenericTypeDefinition(), Name);
            }

            return policy;
        }

        public ResolveDelegateFactory GetFactory()
        {
            // From registration
            ResolveDelegateFactory policy = Registration.Get<ResolveDelegateFactory>();
            if (null != policy) return policy;

            if (Registration is ContainerRegistration registration)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (Type?.GetTypeInfo().IsGenericType ?? false)
#else
                if (Type?.IsGenericType ?? false)
#endif
                {
                    policy = ((UnityContainer)Container).GetFactoryPolicy(Type.GetGenericTypeDefinition(), Name);
                }
                else if (RegistrationType.IsArray) return ArrayResolver.Factory;
                else policy = ((UnityContainer)Container).GetFactoryPolicy(Type);
            }
            else
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (RegistrationType.GetTypeInfo().IsGenericType)
#else
                if (RegistrationType.IsGenericType)
#endif
                {
                    policy = ((UnityContainer)Container).GetFactoryPolicy(RegistrationType.GetGenericTypeDefinition(), Name);
                }
                else if (RegistrationType.IsArray) return ArrayResolver.Factory;
                else policy = ((UnityContainer)Container).GetFactoryPolicy(Type); 
            }

            return policy ?? ((UnityContainer)Container).Defaults.Get<ResolveDelegateFactory>();
        }

        #endregion
    }
}
