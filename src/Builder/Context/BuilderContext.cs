using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public delegate object ExecutePlanDelegate(BuilderStrategy[] chain, ref BuilderContext context);

        #endregion


        #region IResolveContext

        public IUnityContainer Container => Lifetime?.Container;

        public Type Type { get; set; }

        public string Name { get; set; }

        public object Resolve(Type type, string name) => Resolve(type, name,
            (InternalRegistration)((UnityContainer)Container).GetRegistration(type, name));

        #endregion


        #region IPolicyList

        public object Get(Type policyInterface)
        {
            return list.Get(RegistrationType, Name, policyInterface) ?? 
                   Registration.Get(policyInterface);
        }

        public object Get(Type type, string name, Type policyInterface)
        {
            return list.Get(type, name, policyInterface) ??
                   (type != RegistrationType || name != Name
                       ? ((UnityContainer)Container).GetPolicy(type, name, policyInterface)
                       : Registration.Get(policyInterface));
        }

        public void Set(Type policyInterface, object policy)
        {
            list.Set(RegistrationType, Name, policyInterface, policy);
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

        public IPolicySet Registration { get; set; }

        #endregion


        #region Public Properties

        public object Existing { get; set; }

        public ILifetimeContainer Lifetime;

        public SynchronizedLifetimeManager RequiresRecovery;

        public bool BuildComplete;

        public IntPtr Parent;

        public ExecutePlanDelegate ExecutePlan;

        #endregion


        #region Public Methods

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
                    list = list,
                    Overrides = Overrides,
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
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
                case FieldInfo info
                when ReferenceEquals(info, field):
                    return Resolve(field.FieldType, name);

                case DependencyAttribute dependencyAttribute when ReferenceEquals(dependencyAttribute, DependencyAttribute.Instance):
                    return Resolve(field.FieldType, name);

                case OptionalDependencyAttribute optionalAttribute when ReferenceEquals(optionalAttribute, OptionalDependencyAttribute.Instance):
                    try   { return Resolve(field.FieldType, name); }
                    catch { return null; }

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);

                case IResolverFactory factory:
                    var method = factory.GetResolver<BuilderContext>(Type);
                    return method?.Invoke(ref context);
            }

            return value;
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
                case PropertyInfo info
                when ReferenceEquals(info, property):
                    return Resolve(property.PropertyType, name);

                case DependencyAttribute dependencyAttribute when ReferenceEquals(dependencyAttribute, DependencyAttribute.Instance):
                    return Resolve(property.PropertyType, name);

                case OptionalDependencyAttribute optionalAttribute when ReferenceEquals(optionalAttribute, OptionalDependencyAttribute.Instance):
                    try { return Resolve(property.PropertyType, name); }
                    catch { return null; }

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);

                case IResolverFactory factory:
                    var method = factory.GetResolver<BuilderContext>(Type);
                    return method?.Invoke(ref context);
            }

            return value;
        }

        #endregion
    }
}
