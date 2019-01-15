using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class MethodProcessor
    {
        #region Overrides

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object resolvers)
        {
            var parameterResolvers = CreateParameterResolvers(info.GetParameters(), resolvers).ToArray();
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
    }
}
