using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Resolution;
using Unity.Lifetime;
using Unity.Registration;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        #region Public Methods

        public object? Resolve(Type type, string? name, InternalRegistration registration)
        {
            unsafe
            {
                var thisContext = this;
                var containerRegistration = registration as ContainerRegistration;
                var container = registration.Get(typeof(LifetimeManager)) is ContainerControlledLifetimeManager manager
                              ? ((UnityContainer)manager.Scope!).LifetimeContainer
                              : Lifetime;

                var context = new BuilderContext
                {
                    Lifetime = container,
                    Registration = registration,
                    RegistrationType = type,
                    Name = name,
                    Type = null != containerRegistration ? containerRegistration.Type : type,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                return ExecutePlan(registration.BuildChain ?? ((UnityContainer)context.Container)._strategiesChain, ref context);
            }
        }

        #endregion


        #region Parameter

        public object? Override(ParameterInfo parameter, string? name, object? value)
        {
            if (null == Overrides) return value;

            unsafe
            {
                for (var index = Overrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = Overrides[index];

                    // Check if this parameter is overridden
                    if (resolverOverride is IEquatable<ParameterInfo> comparer && comparer.Equals(parameter))
                    {

                        var thisContext = this;
                        var context = new BuilderContext
                        {
                            Lifetime = Lifetime,
                            Registration = Registration,
                            RegistrationType = RegistrationType,
                            Name = name,
                            Type = parameter.ParameterType,
                            ExecutePlan = ExecutePlan,
                            ResolvePlan = ResolvePlan,
                            List = List,
                            Overrides = Overrides,
                            DeclaringType = Type,
#if !NET40
                            Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                        };

                        // Check if itself is a value 
                        if (resolverOverride is IResolve resolverPolicy)
                            return resolverPolicy.Resolve(ref context);

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(parameter.ParameterType);
                        if (null != resolveDelegate) return resolveDelegate(ref context);
                    }
                }
            }

            return value;
        }

        public object? Resolve(ParameterInfo parameter, string? name, ResolveDelegate<BuilderContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    Lifetime = Lifetime,
                    Registration = Registration,
                    RegistrationType = RegistrationType,
                    Name = name,
                    Type = parameter.ParameterType,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

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

                return resolver(ref context);
            }
        }

        public object? ValidatingResolve(ParameterInfo parameter, string? name, ResolveDelegate<BuilderContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    Lifetime = Lifetime,
                    Registration = Registration,
                    RegistrationType = RegistrationType,
                    Name = name,
                    Type = parameter.ParameterType,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                try
                {
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

                    return resolver(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), parameter);
                    throw;
                }
            }
        }

        #endregion


        #region Field

        public object? Override(FieldInfo field, string? name, object? value)
        {
            if (null == Overrides) return value;

            unsafe
            {
                for (var index = Overrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = Overrides[index];

                    // Check if this parameter is overridden
                    if (resolverOverride is IEquatable<FieldInfo> comparer && comparer.Equals(field))
                    {

                        var thisContext = this;
                        var context = new BuilderContext
                        {
                            Lifetime = Lifetime,
                            Registration = Registration,
                            RegistrationType = RegistrationType,
                            Name = name,
                            Type = field.FieldType,
                            ExecutePlan = ExecutePlan,
                            ResolvePlan = ResolvePlan,
                            List = List,
                            Overrides = Overrides,
                            DeclaringType = Type,
#if !NET40
                            Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                        };

                        // Check if itself is a value 
                        if (resolverOverride is IResolve resolverPolicy)
                            return resolverPolicy.Resolve(ref context);

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(field.FieldType);
                        if (null != resolveDelegate) return resolveDelegate(ref context);
                    }
                }
            }

            return value;
        }

        public object? Resolve(FieldInfo field, string? name, ResolveDelegate<BuilderContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    Lifetime = Lifetime,
                    Registration = Registration,
                    RegistrationType = RegistrationType,
                    Name = name,
                    Type = field.FieldType,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

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

                return resolver(ref context);
            }
        }

        public object? ValidatingResolve(FieldInfo field, string? name, ResolveDelegate<BuilderContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    Lifetime = Lifetime,
                    Registration = Registration,
                    RegistrationType = RegistrationType,
                    Name = name,
                    Type = field.FieldType,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                try
                {
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

                    return resolver(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), field);
                    throw;
                }
            }
        }

        #endregion


        #region Property

        public object? Override(PropertyInfo property, string? name, object? value)
        {
            if (null == Overrides) return value;

            unsafe
            {
                for (var index = Overrides.Length - 1; index >= 0; --index)
                {
                    var resolverOverride = Overrides[index];

                    // Check if this parameter is overridden
                    if (resolverOverride is IEquatable<PropertyInfo> comparer && comparer.Equals(property))
                    {

                        var thisContext = this;
                        var context = new BuilderContext
                        {
                            Lifetime = Lifetime,
                            Registration = Registration,
                            RegistrationType = RegistrationType,
                            Name = name,
                            Type = property.PropertyType,
                            ExecutePlan = ExecutePlan,
                            ResolvePlan = ResolvePlan,
                            List = List,
                            Overrides = Overrides,
                            DeclaringType = Type,
#if !NET40
                            Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                        };

                        // Check if itself is a value 
                        if (resolverOverride is IResolve resolverPolicy)
                            return resolverPolicy.Resolve(ref context);

                        // Try to create value
                        var resolveDelegate = resolverOverride.GetResolver<BuilderContext>(property.PropertyType);
                        if (null != resolveDelegate) return resolveDelegate(ref context);
                    }
                }
            }

            return value;
        }

        public object? Resolve(PropertyInfo property, string? name, ResolveDelegate<BuilderContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    Lifetime = Lifetime,
                    Registration = Registration,
                    RegistrationType = RegistrationType,
                    Name = name,
                    Type = property.PropertyType,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

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

                return resolver(ref context);
            }
        }

        public object? ValidatingResolve(PropertyInfo property, string? name, ResolveDelegate<BuilderContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new BuilderContext
                {
                    Lifetime = Lifetime,
                    Registration = Registration,
                    RegistrationType = RegistrationType,
                    Name = name,
                    Type = property.PropertyType,
                    ExecutePlan = ExecutePlan,
                    ResolvePlan = ResolvePlan,
                    List = List,
                    Overrides = Overrides,
                    DeclaringType = Type,
#if !NET40
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                };

                try
                {
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

                    return resolver(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), property);
                    throw;
                }
            }
        }

        #endregion
    }
}
