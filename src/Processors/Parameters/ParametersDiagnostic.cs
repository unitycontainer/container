using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo>
    {
        #region Diagnostic Parameter Factory

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateDiagnosticParameterResolvers(ParameterInfo[] parameters, object injectors = null)
        {
            object[] resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers
                             ? FromAttribute(parameter)
                             : PreProcessResolver(parameter, resolvers[i]);

                yield return CreateDiagnosticParameterResolver(parameter, resolver);
            }
        }

        protected virtual ResolveDelegate<BuilderContext> CreateDiagnosticParameterResolver(ParameterInfo parameter, object resolver)
        {
            // TODO: Add diagnostic to parameters

            var resolverDelegate = CreateParameterResolver(parameter, resolver);

            return (ref BuilderContext context) =>
            {
                try
                {
                    return resolverDelegate(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), parameter);
                    throw;
                }
            };
        }


        #endregion
    }
}
