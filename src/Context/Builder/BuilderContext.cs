using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
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

        internal IPolicyList List { get; set; }
        public delegate object? ResolvePlanDelegate(ref BuilderContext context, ResolveDelegate<BuilderContext> resolver);

        #endregion


        #region IResolveContext

        public IUnityContainer Container => ContainerContext.Container;

        public Type Type { get; set; }

        public string? Name => Registration?.Name;

        public object? Resolve(Type type, string? name)
        {
            // Process overrides if any
            if (0 < Overrides.Length)
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

                        return DependencyResolvePipeline(ref context, resolverPolicy.Resolve);
                    }
                }
            }

            return Resolve(type, ((UnityContainer)Container).GetRegistration(type, name));
        }

        #endregion


        #region IPolicyList

        public object? Get(Type policyInterface)
        {
            return List?.Get(Type, Name, policyInterface) ?? 
                Registration?.Get(policyInterface);
        }

        public object? Get(Type type, Type policyInterface)
        {
            return ContainerContext.Get(type, policyInterface);
        }

        public object? Get(Type type, string? name, Type policyInterface)
        {
            return List?.Get(type, name, policyInterface) ?? (type != Type || name != Name
                ? ContainerContext.Get(type, name, policyInterface)
                : Registration?.Get(policyInterface));
        }

        public void Set(Type policyInterface, object policy)
        {
            List.Set(Type, Name, policyInterface, policy);
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


        #region Public Properties

        public Regex Regex;

        public bool IsAsync { get; set; }


        public ResolverOverride[] Overrides;

        public object? Existing { get; set; }

        public IRegistration? Registration { get; set; }

        public ContainerContext ContainerContext { get; set; }

        public Type? DeclaringType;
#if !NET40
        public IntPtr Parent;
#endif
        public ResolvePlanDelegate DependencyResolvePipeline => ContainerContext.Container.DependencyResolvePipeline;

        public ResolveDelegate<BuilderContext> Pipeline
        {
            get
            {
                if (null != Registration?.Pipeline) return Registration.Pipeline;

                Debug.Assert(null != Registration);

                lock (Registration)
                {
                    // Double check
                    if (null != Registration.Pipeline) return Registration.Pipeline;

                    // Create a pipeline
                    var context = this;
                    PipelineBuilder builder = new PipelineBuilder(ref context);
                    Registration.Pipeline = builder.Pipeline() ??
                        throw new InvalidOperationException($"Failed to create pipeline for registration: {Registration}");
                }

                return Registration.Pipeline;
            }
        }

        #endregion


        #region Member Resolution

        public object? Resolve(ParameterInfo parameter, object? value)
        {
            var context = this;

            // Process overrides if any
            if (0 < Overrides.Length)
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
                            return DependencyResolvePipeline(ref context, resolverPolicy.Resolve);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(parameter.ParameterType);
                        if (null != resolveDelegate)
                        {
                            return DependencyResolvePipeline(ref context, resolveDelegate);
                        }
                    }
                }
            }

            // Resolve from injectors
            switch (value)
            {
                case ParameterInfo info
                when ReferenceEquals(info, parameter):
                    return Resolve(parameter.ParameterType, (string?)null);

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);
            }

            return value;
        }

        public object? Resolve(PropertyInfo property, object? value)
        {
            var context = this;

            // Process overrides if any
            if (0 < Overrides.Length)
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
                            return DependencyResolvePipeline(ref context, resolverPolicy.Resolve);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(property.PropertyType);
                        if (null != resolveDelegate)
                        {
                            return DependencyResolvePipeline(ref context, resolveDelegate);
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
                    catch (Exception ex) when (!(ex is CircularDependencyException))
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
            if (0 < Overrides.Length)
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
                            return DependencyResolvePipeline(ref context, resolverPolicy.Resolve);
                        }

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(field.FieldType);
                        if (null != resolveDelegate)
                        {
                            return DependencyResolvePipeline(ref context, resolveDelegate);
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
                    catch (Exception ex) when (!(ex is CircularDependencyException))
                    {
                        return null;
                    }

                case ResolveDelegate<BuilderContext> resolver:
                    return resolver(ref context);
            }

            return value;
        }

        #endregion


        #region Public Methods

        public object? Resolve(Type type)
        {
            // Process overrides if any
            if (0 < Overrides.Length)
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

                        return DependencyResolvePipeline(ref context, resolverPolicy.Resolve);
                    }
                }
            }

            var key = new HashKey(type, Name);
            return Resolve(ContainerContext.Container.GetPipeline(ref key));
        }

        public object? Resolve(Type type, IRegistration registration)
        {
            if (ReferenceEquals(Registration, registration)) throw new CircularDependencyException(type, registration.Name);

            unsafe
            {
                var thisContext = this;
                var container = registration.LifetimeManager is ContainerControlledLifetimeManager containerControlled 
                    ? (ContainerContext)containerControlled.Scope 
                    : ContainerContext;

                var context = new BuilderContext
                {
                    ContainerContext = container,
                    Registration = registration,
                    IsAsync = IsAsync,
                    Type = type,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                var manager = registration.LifetimeManager switch
                {
                    null                        => TransientLifetimeManager.Instance,
                    PerResolveLifetimeManager _ => (LifetimeManager?)context.Get(typeof(LifetimeManager)) ?? 
                                                    TransientLifetimeManager.Instance,
                    _                           => registration.LifetimeManager
                };

                // Check if already got value
                var value = manager.GetValue(ContainerContext.Lifetime);
                if (LifetimeManager.NoValue != value) return value;

                if (registration.LifetimeManager is SynchronizedLifetimeManager synchronized)
                {
                    try
                    {
                        value = context.Pipeline(ref context);
                    }
                    catch
                    {
                        synchronized.Recover();
                        throw;
                    }
                }
                else
                    value = context.Pipeline(ref context);

                manager.SetValue(value, ContainerContext.Lifetime);
                return value;
            }
        }

        public object? Resolve(ResolveDelegate<BuilderContext> pipeline)
        {
            // Check if already created and acquire a lock if not
            var manager = pipeline.Target as LifetimeManager;
            if (null != manager)
            {
                // Make blocking check for result
                var value = manager.Get(ContainerContext.Lifetime);
                if (LifetimeManager.NoValue != value) return value;
            }

            // Setup Context
            var synchronized = manager as SynchronizedLifetimeManager;
            var thisContext = this;

            try
            {
                // Execute pipeline
                var value = pipeline(ref thisContext);
                manager?.Set(value, ContainerContext.Lifetime);
                return value;
            }
            catch when (null != synchronized)
            {
                synchronized.Recover();
                throw;
            }
        }

        #endregion
    }
}
