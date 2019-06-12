using System.Collections.Generic;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public abstract partial class ParametersPipeline<TMemberInfo>
    {
        protected virtual IEnumerable<ResolveDelegate<PipelineContext>> CreateParameterResolvers(ParameterInfo[] parameters, object? injectors = null)
        {
            object[]? resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers
                             ? FromAttribute(parameter)
                             : PreProcessResolver(parameter, resolvers[i]);
#if NET40
                if (parameter.DefaultValue is DBNull)
#else
                if (!parameter.HasDefaultValue)
#endif
                {
                    // Plain vanilla case
                    yield return (ref PipelineContext context) => context.Resolve(parameter, resolver);
                }
                else
                {
                    // Check if has default value
#if NET40
                    var defaultValue = !(parameter.DefaultValue is DBNull) ? parameter.DefaultValue : null;
#else
                    var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
#endif
                    yield return (ref PipelineContext context) =>
                    {
                        try
                        {
                            return context.Resolve(parameter, resolver);
                        }
                        catch
                        {
                            return defaultValue;
                        }
                    };
                }
            }
        }
    }
}
