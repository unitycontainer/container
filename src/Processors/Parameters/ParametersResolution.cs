using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo>
    {
        #region Parameter Factory

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateParameterResolvers(ParameterInfo[] parameters, object injectors = null)
        {
            object[] resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers 
                             ? FromAttribute(parameter) 
                             : PreProcessResolver(parameter, resolvers[i]);

                if (!parameter.HasDefaultValue)
                {
                    // Plain vanilla case
                    yield return (ref BuilderContext context) => context.Resolve(parameter, resolver);
                }
                else
                {
                    // Check if has default value
                    var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                    yield return (ref BuilderContext context) =>
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

        #endregion
    }
}
