using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected object? BuildParameter(ref PipelineContext context, ParameterInfo parameter)
        {
            ResolverOverride[] overrides;

            if (null != (overrides = context.Overrides))
            { 
                for (var index = overrides.Length - 1; index >= 0; --index)
                {
                    var @override = overrides[index];

                    // Check if this parameter is overridden
                    if (@override is IEquatable<ParameterInfo> comparer && comparer.Equals(parameter))
                    {
                        var local = new PipelineContext(ref context, parameter, @override);

                        return @override.Resolve(ref local);
                    }
                }
            
            }


            // From annotation
            return null;
        }


        private object? ReinterpretValue<TContext>(ref TContext context, object? value)
            where TContext : IResolveContext
        {
            return value switch
            {
                IResolve resolve => ReinterpretValue(ref context, resolve.Resolve(ref context)),
                IResolverFactory<Type> factory => ReinterpretValue(ref context, factory.GetResolver<TContext>(context.Type)
                                                                                       .Invoke(ref context)),
                _ => value,
            };
        }

        protected object? BuildParameter(ref PipelineContext context, ParameterInfo parameter, object? value)
        {
            ResolverOverride[] overrides;

            if (null != (overrides = context.Overrides))
            {
                for (var index = overrides.Length - 1; index >= 0; --index)
                {
                    var @override = overrides[index];

                    // Check if this parameter is overridden
                    if (@override is IEquatable<ParameterInfo> comparer && comparer.Equals(parameter))
                    {
                        // 

                        return @override;
                    }
                }

            }

            // From value
            return value;

            // From annotation
            //return null;
        }

        protected object? ProcessValue(ref PipelineContext context, object? value)
        {
            return null;
        }
    }
}
