using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public class MethodDiagnostic : MethodProcessor
    {
        #region Constructors

        public MethodDiagnostic(IPolicySet policySet) 
            : base(policySet)
        {
        }

        #endregion


        #region Overrides

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object resolvers)
        {
            var parameterResolvers = (ResolveDelegate<BuilderContext>[])resolvers;
            return (ref BuilderContext c) =>
            {
                try
                {
                    if (null == c.Existing) return c.Existing;

                    var parameters = new object[parameterResolvers.Length];
                    for (var i = 0; i < parameters.Length; i++)
                        parameters[i] = parameterResolvers[i](ref c);

                    info.Invoke(c.Existing, parameters);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(info, null);
                    throw;
                }

                return c.Existing;
            };
        }

        #endregion
    }
}
