using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public abstract partial class MethodBaseProcessor<TMemberInfo>
    {
        protected virtual object?[] BuildUpParameters(ref ResolutionContext context, ParameterInfo[] parameters)
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
                }
                else
                {
                    // BuildUp ParameterInfo
                    values[i] = BuildUpParameterInfo(ref context, parameter);
                }
            }

            return values;
        }
        protected virtual object?[] BuildUpParameters(ref ResolutionContext context, ParameterInfo[] parameters, object?[] data)
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
                var value = data[i];

                // BuildUp ParameterInfo
                values[i] = BuildUpParameterInfo(ref context, parameter);
            }

            return values;
        }

        protected virtual object? BuildUpParameterInfo(ref ResolutionContext context, ParameterInfo parameter)
        {


            var info = GetParameterDependencyInfo(parameter);


            return null;
        }

        protected virtual object? BuildUpParameterInfo(ref ResolutionContext context, ParameterInfo info, object? value)
        {

            return null;
        }
    }
}
