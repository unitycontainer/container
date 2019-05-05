using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Composition;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Strategies;
using static Unity.UnityContainer;

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

        public ResolverOverride[]? Overrides;
        internal IPolicyList List;

        public delegate object? ExecutePlanDelegate(BuilderStrategy[] chain, ref BuilderContext context);
        public delegate object? ResolvePlanDelegate(ref BuilderContext context, ResolveDelegate<BuilderContext> resolver);

        #endregion


        #region IResolveContext

        public IUnityContainer Container => ContainerContext.Container;

        public Type Type { get; set; }

        public string? Name { get; set; }

        public object? Resolve(Type type, string? name)
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

            if (ContainerContext.Get(type, name, typeof(CompositionDelegate)) is CompositionDelegate factory)
                return factory((UnityContainer)Container, null);

            return Resolve(type, name, ((UnityContainer)Container).GetRegistration(type, name));
        }

        #endregion


        #region IPolicyList

        public object? Get(Type policyInterface)
        {
            return List.Get(RegistrationType, Name, policyInterface) ?? Registration.Get(policyInterface);
        }

        public object? Get(Type type, Type policyInterface)
        {
            return ContainerContext.Get(type, policyInterface);
        }

        public object? Get(Type type, string? name, Type policyInterface)
        {
            return List.Get(type, name, policyInterface) ??
                   (type != RegistrationType || name != Name
                       ? ContainerContext.Get(type, name, policyInterface)
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

        public void Set(Type type, string? name, Type policyInterface, object policy)
        {
            List.Set(type, name, policyInterface, policy);
        }

        public void Clear(Type type, string? name, Type policyInterface)
        {
            List.Clear(type, name, policyInterface);
        }

        #endregion


        #region Registration

        // TODO: Remove
        public Type RegistrationType { get; set; }

        public ImplicitRegistration Registration { get; set; }

        #endregion


        #region Public Properties

        public object? Existing { get; set; }

        public ContainerContext ContainerContext { get; set; }

        public SynchronizedLifetimeManager? RequiresRecovery;

        public bool BuildComplete;

        public Type? DeclaringType;
#if !NET40
        public IntPtr Parent;
#endif
        public ResolveDelegate<BuilderContext> Compose;

        public ResolvePlanDelegate ResolvePlan;

        #endregion


        #region Resolve Methods


        public object? Resolve()
        {
            if (null == Type) return null;

            // Process overrides if any
            if (null != Overrides)
            {
                NamedType namedType = new NamedType
                {
                    Type = Type,
                    Name = Name
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

            return Resolve(Type, Name, ((UnityContainer)Container).GetRegistration(Type, Name));
        }

        public object? Resolve(Type type)
        {
            // Process overrides if any
            if (null != Overrides)
            {
                NamedType namedType = new NamedType
                {
                    Type = type,
                    Name = Name
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

            return Resolve(type, Name, ((UnityContainer)Container).GetRegistration(type, Name));
        }

        public object? Resolve(ParameterInfo parameter, object? value)
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

        public object? Resolve(PropertyInfo property, object? value)
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

        public object? Resolve(FieldInfo field, object? value)
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

        public object? Resolve(Type type, string? name, ImplicitRegistration registration)
        {
            if (ReferenceEquals(Registration, registration)) throw new CircularDependencyException();

            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    ContainerContext = ContainerContext,
                    Registration = registration,
                    RegistrationType = type,
                    Name = name,
                    Type = type,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
                    Compose = Compose,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                return Compose(ref context);
            }
        }

        #endregion
    }
}
