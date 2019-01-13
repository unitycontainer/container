using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class MethodBaseProcessor<TMemberInfo>
    {
        #region Overrides

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object resolvers)
        {
            var parameterResolvers = (ResolveDelegate<BuilderContext>[])resolvers;
            return (ref BuilderContext c) =>
            {
                if (null == c.Existing) return c.Existing;

                var parameters = new object[parameterResolvers.Length];
                for (var i = 0; i < parameters.Length; i++)
                    parameters[i] = parameterResolvers[i](ref c);

                info.Invoke(c.Existing, parameters);

                return c.Existing;
            };
        }

        #endregion


        #region Parameter Factory

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateParameterResolvers(ParameterInfo[] parameters, object[] injectors = null)
        {
            object[] resolvers = null != injectors && 0 == injectors.Length ? null : injectors;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers 
                             ? FromAttribute(parameter) 
                             : PreProcessResolver(parameter, resolvers[i]);

                yield return CreateParameterResolver(parameter, resolver);
            }
        }

        protected virtual ResolveDelegate<BuilderContext> CreateParameterResolver(ParameterInfo parameter, object resolver)
        {
            // Check if has default value
            if (!parameter.HasDefaultValue)
            {
                // Plain vanilla case
                return (ref BuilderContext context) => context.Resolve(parameter, resolver);
            }
            else
            {
                // Check if has default value
                var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                return (ref BuilderContext context) =>
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

        #endregion
    }
}
