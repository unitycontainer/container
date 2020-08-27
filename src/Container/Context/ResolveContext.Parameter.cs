using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
#if NETSTANDARD1_6 || NETCOREAPP1_0 || NETSTANDARD2_0 || NETSTANDARD2_1
    public partial class ResolveContext
#else
    public partial struct ResolveContext
#endif
    {
        #region Override

        public object? Override(ParameterInfo parameter, string? name, object? value)
        {
            if (null == Overrides) return value;

            for (var index = Overrides.Length - 1; index >= 0; --index)
            {
                var resolverOverride = Overrides[index];

                // Check if this parameter is overridden
                if (resolverOverride is IEquatable<ParameterInfo> comparer && comparer.Equals(parameter))
                {
                    var context = StartActivity(parameter, name);

                    // Check if itself is a value 
                    if (resolverOverride is IResolve resolverPolicy)
                        return resolverPolicy.Resolve(ref context);

                    // Try to create value
                    var resolveDelegate = resolverOverride.GetResolver<ResolveContext>(parameter.ParameterType);
                    if (null != resolveDelegate) return resolveDelegate(ref context);
                }
            }

            return value;
        }

        #endregion


        #region Resolve

        public object? Resolve(ParameterInfo parameter)
        {
            var attribute = parameter.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                                ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<ResolveContext>(parameter);

            return Resolve(parameter, attribute.Name, resolver);
        }

        public object? Resolve(ParameterInfo parameter, object? data)
        {
            var attribute = parameter.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                                ?? DependencyAttribute.Instance;
            ResolveDelegate<ResolveContext>? resolver = PreProcessResolver(parameter, attribute, data);

            if (null == resolver)
                return Override(parameter, attribute.Name, data);
            else
                return Resolve(parameter, attribute.Name, resolver);
        }

        public object? Resolve(ParameterInfo parameter, string? name, ResolveDelegate<ResolveContext> resolver)
        {
            var context = StartActivity(parameter, name);

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
                        var resolveDelegate = resolverOverride.GetResolver<ResolveContext>(parameter.ParameterType);
                        if (null != resolveDelegate)
                        {
                            return resolveDelegate(ref context);
                        }
                    }
                }
            }
            
            return resolver(ref context);
        }

        #endregion


        #region Implementation

        ResolveDelegate<ResolveContext>? PreProcessResolver(ParameterInfo info, DependencyResolutionAttribute attribute, object? data)
            => data switch
            {
                IResolve policy => policy.Resolve,
                IResolverFactory<ParameterInfo> fieldFactory => fieldFactory.GetResolver<ResolveContext>(info),
                IResolverFactory<Type> typeFactory => typeFactory.GetResolver<ResolveContext>(info.ParameterType),
                Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<ResolveContext>(type),
                _ => null
            };

        #endregion
    }
}
