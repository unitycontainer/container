using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity
{
    public partial struct PipelineContext
    {
        #region Fields


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
                        var context = new PipelineContext
                        {
                            ContainerContext = ContainerContext,
                            List = List,
                            Name = name,
                            Type = parameter.ParameterType,
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
                        var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(parameter.ParameterType);
                        if (null != resolveDelegate) return resolveDelegate(ref context);
                    }
                }
            }

            return value;
        }

        public object? OverrideDiagnostic(ParameterInfo parameter, string? name, object? value)
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
                        var context = new PipelineContext
                        {
                            ContainerContext = ContainerContext,
                            List = List,
                            Name = name,
                            Type = parameter.ParameterType,
                            Overrides = Overrides,
                            DeclaringType = Type,
#if !NET40
                            Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                        };

                        try
                        {
                            // Check if itself is a value 
                            if (resolverOverride is IResolve resolverPolicy)
                                return resolverPolicy.Resolve(ref context);

                            // Try to create value
                            var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(parameter.ParameterType);
                            if (null != resolveDelegate) return resolveDelegate(ref context);
                        }
                        catch (Exception ex)
                        {
                            ex.Data.Add(Guid.NewGuid(), parameter);
                            throw;
                        }
                    }
                }
            }

            return value;
        }

        public object? Resolve(ParameterInfo parameter, string? name, ResolveDelegate<PipelineContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new PipelineContext
                {
                    ContainerContext = ContainerContext,
                    List = List,
                    Name = name,
                    Type = parameter.ParameterType,
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
                                return resolverPolicy.Resolve(ref context);
                            }

                            // Try to create value
                            var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(parameter.ParameterType);
                            if (null != resolveDelegate)
                            {
                                return resolveDelegate(ref context);
                            }
                        }
                    }
                }

                return resolver(ref context);
            }
        }

        public object? ResolveDiagnostic(ParameterInfo parameter, string? name, ResolveDelegate<PipelineContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new PipelineContext
                {
                    ContainerContext = ContainerContext,
                    List = List,
                    Name = name,
                    Type = parameter.ParameterType,
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
                                    return resolverPolicy.Resolve(ref context);
                                }

                                // Try to create value
                                var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(parameter.ParameterType);
                                if (null != resolveDelegate)
                                {
                                    return resolveDelegate(ref context);
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
                        var context = new PipelineContext
                        {
                            ContainerContext = ContainerContext,
                            List = List,
                            Name = name,
                            Type = field.FieldType,
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
                        var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(field.FieldType);
                        if (null != resolveDelegate) return resolveDelegate(ref context);
                    }
                }
            }

            return value;
        }

        public object? OverrideDiagnostic(FieldInfo field, string? name, object? value)
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
                        var context = new PipelineContext
                        {
                            ContainerContext = ContainerContext,
                            List = List,
                            Name = name,
                            Type = field.FieldType,
                            Overrides = Overrides,
                            DeclaringType = Type,
#if !NET40
                            Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                        };

                        try
                        {
                            // Check if itself is a value 
                            if (resolverOverride is IResolve resolverPolicy)
                                return resolverPolicy.Resolve(ref context);

                            // Try to create value
                            var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(field.FieldType);
                            if (null != resolveDelegate) return resolveDelegate(ref context);
                        }
                        catch (Exception ex)
                        {
                            // TODO: add resolverOverride
                            ex.Data.Add(Guid.NewGuid(), field);
                            throw;
                        }
                    }
                }
            }

            return value;
        }

        public object? Resolve(FieldInfo field, string? name, ResolveDelegate<PipelineContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new PipelineContext
                {
                    ContainerContext = ContainerContext,
                    List = List,
                    Name = name,
                    Type = field.FieldType,
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
                                return resolverPolicy.Resolve(ref context);
                            }

                            // Try to create value
                            var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(field.FieldType);
                            if (null != resolveDelegate)
                            {
                                return resolveDelegate(ref context);
                            }
                        }
                    }
                }

                return resolver(ref context);
            }
        }

        public object? ResolveDiagnostic(FieldInfo field, string? name, ResolveDelegate<PipelineContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new PipelineContext
                {
                    ContainerContext = ContainerContext,
                    List = List,
                    Name = name,
                    Type = field.FieldType,
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
                                    return resolverPolicy.Resolve(ref context);
                                }

                                // Try to create value
                                var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(field.FieldType);
                                if (null != resolveDelegate)
                                {
                                    return resolveDelegate(ref context);
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
                        var context = new PipelineContext
                        {
                            ContainerContext = ContainerContext,
                            List = List,
                            Name = name,
                            Type = property.PropertyType,
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
                        var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(property.PropertyType);
                        if (null != resolveDelegate) return resolveDelegate(ref context);
                    }
                }
            }

            return value;
        }

        public object? OverrideDiagnostic(PropertyInfo property, string? name, object? value)
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
                        var context = new PipelineContext
                        {
                            ContainerContext = ContainerContext,
                            List = List,
                            Name = name,
                            Type = property.PropertyType,
                            Overrides = Overrides,
                            DeclaringType = Type,
#if !NET40
                            Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
#endif
                        };

                        try
                        {
                            // Check if itself is a value 
                            if (resolverOverride is IResolve resolverPolicy)
                                return resolverPolicy.Resolve(ref context);

                            // Try to create value
                            var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(property.PropertyType);
                            if (null != resolveDelegate) return resolveDelegate(ref context);
                        }
                        catch (Exception ex)
                        {
                            // TODO: add resolverOverride
                            ex.Data.Add(Guid.NewGuid(), property);
                            throw;
                        }
                    }
                }
            }

            return value;
        }

        public object? Resolve(PropertyInfo property, string? name, ResolveDelegate<PipelineContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new PipelineContext
                {
                    ContainerContext = ContainerContext,
                    List = List,
                    Name = name,
                    Type = property.PropertyType,
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
                                return resolverPolicy.Resolve(ref context);
                            }

                            // Try to create value
                            var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(property.PropertyType);
                            if (null != resolveDelegate)
                            {
                                return resolveDelegate(ref context);
                            }
                        }
                    }
                }

                return resolver(ref context);
            }
        }

        public object? ResolveDiagnostic(PropertyInfo property, string? name, ResolveDelegate<PipelineContext> resolver)
        {
            unsafe
            {
                var thisContext = this;
                var context = new PipelineContext
                {
                    ContainerContext = ContainerContext,
                    List = List,
                    Name = name,
                    Type = property.PropertyType,
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
                                    return resolverPolicy.Resolve(ref context);
                                }

                                // Try to create value
                                var resolveDelegate = resolverOverride.GetResolver<PipelineContext>(property.PropertyType);
                                if (null != resolveDelegate)
                                {
                                    return resolveDelegate(ref context);
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
