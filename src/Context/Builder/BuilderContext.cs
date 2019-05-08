using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
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

        internal IPolicyList List;

        public delegate object? ResolvePlanDelegate(ref BuilderContext context, ResolveDelegate<BuilderContext> resolver);

        #endregion


        #region IResolveContext

        public IUnityContainer Container => ContainerContext.Container;

        public Type Type { get; set; }

        public string? Name => Registration.Name;

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
            return List.Get(Type, Name, policyInterface) ?? 
                Registration.Get(policyInterface);
        }

        public object? Get(Type type, Type policyInterface)
        {
            return ContainerContext.Get(type, policyInterface);
        }

        public object? Get(Type type, string? name, Type policyInterface)
        {
            return List.Get(type, name, policyInterface) ??
                   (type != Type || name != Name
                       ? ContainerContext.Get(type, name, policyInterface)
                       : Registration.Get(policyInterface));
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

        public bool Async;

        public ResolverOverride[] Overrides;

        public object? Existing { get; set; }

        public ImplicitRegistration Registration { get; set; }

        public ContainerContext ContainerContext { get; set; }

        public Type? DeclaringType;
#if !NET40
        public IntPtr Parent;
#endif
        public ResolveDelegate<BuilderContext> Pipeline
        {
            get
            {
                ResolveDelegate<BuilderContext>? pipeline = Registration.Pipeline;

                if (null == pipeline)
                {
                    bool locked = false;
                    try
                    {
                        // We will wait reasonable time to acquire the lock
                        Monitor.TryEnter(Registration, DefaultTimeOut, ref locked);

                        // Double check lock
                        if (null != Registration.Pipeline) return Registration.Pipeline;

                        // Create a pipeline
                        var context = this;
                        PipelineBuilder builder = new PipelineBuilder(ref context);
                        pipeline = builder.Pipeline() ?? throw new InvalidOperationException("Invalid Pipeline");

                        // Save is lock is acquired
                        if (locked) Registration.Pipeline = pipeline;
                    }
                    finally
                    {
                        // Release, if lock is acquired
                        if (locked) Monitor.Exit(Registration);
                    }
                }

                return pipeline;
            }
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

            return Resolve(type, ((UnityContainer)Container).GetRegistration(type, Name));
        }

        public object? Resolve(Type type, ImplicitRegistration registration)
        {
            if (ReferenceEquals(Registration, registration)) throw new CircularDependencyException();

            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    ContainerContext = ContainerContext,
                    Registration = registration,
                    Type = type,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                return context.Pipeline(ref context);
            }
        }

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
    }
}
