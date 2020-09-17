using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MethodBaseProcessor<TMemberInfo>
    {
        protected virtual object?[] BuildUp(ref ResolutionContext context, ParameterInfo[] parameters)
        {
            ResolutionContext local = context.CreateChildContext();

            var values = new object?[parameters.Length];

            // Resolve dependencies
            for (var i = 0; i < parameters.Length; i++)
            {
                // Parameter
                local.Data = parameters[i];
                ref var value = ref values[i];

                // Try override first
                value = Override(ref local);
                if (!ReferenceEquals(RegistrationManager.NoValue, value)) continue;

                // Get Dependecy Info from annotations
                var dependency = GetParameterDependencyInfo((ParameterInfo)local.Data!);

                // BuildUp ParameterInfo
                values[i] = BuildUpParameterInfo(ref local);
                if (null != dependency.Data)
                { 
                    
                }
            }

            return values;
        }

        protected virtual object?[] BuildUp(ref ResolutionContext context, ParameterInfo[] parameters, object?[]? data = null)
        {
            var values = new object?[parameters.Length];

            // Resolve dependencies
            for (var i = 0; i < parameters.Length; i++)
            {
                ResolverOverride? @override;
                var parameter = parameters[i];

                // Try override first
                if (null != (@override = context.GetOverride(parameter)))
                {
                    // Check if itself is a value 
                    if (@override is IResolve resolverPolicy)
                        values[i] = resolverPolicy.Resolve(ref context);

                    // Try to create value
                    var resolveDelegate = @override.GetResolver<ResolutionContext>(parameter.ParameterType);
                    values[i] = resolveDelegate(ref context);

                    continue;
                }

                // Injected data
                if (null != data)
                { 
                    var mamber = data[i];
                }

                // Get Dependecy Info from annotations
                var info = GetParameterDependencyInfo(parameter);

                // BuildUp ParameterInfo
                values[i] = BuildUpParameterInfo(ref context);
            }

            return values;
        }

        protected virtual object? BuildUpParameterInfo(ref ResolutionContext context)
        {
            var parameter = (ParameterInfo)context.Data!;


            var info = GetParameterDependencyInfo(parameter);


            return null;
        }

        #region Implementation

        protected virtual object? Override(ref ResolutionContext context)
        {
            var parameter = (ParameterInfo)context.Data!;
            var overrides = context.Overrides;

            // Process overrides if any
            if (null == overrides || 0 == overrides.Length) return RegistrationManager.NoValue;

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var resolverOverride = overrides[index];

                // Check if this parameter is overridden
                if (resolverOverride is IEquatable<ParameterInfo> comparable && comparable.Equals(parameter))
                { 
                    // Check if itself is a value 
                    if (resolverOverride is IResolve resolverPolicy)
                        return resolverPolicy.Resolve(ref context);

                    // Try to create value
                    var resolveDelegate = resolverOverride.GetResolver<ResolutionContext>(parameter.ParameterType);
                    return resolveDelegate(ref context);
                }
            }

            return RegistrationManager.NoValue;
        }

        #endregion
    }
}
