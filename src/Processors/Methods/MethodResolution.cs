using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class MethodProcessor
    {
        #region Building Resolver

        protected override ResolveDelegate<BuilderContext> ResolverFromMemberInfo(MethodInfo info)
        {
            ValidateMethod(info);

            var parameterResolvers = CreateParameterResolvers(info.GetParameters()).ToArray();
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

        protected override ResolveDelegate<BuilderContext> ResolverFromMemberInfo(MethodInfo info, object[] resolvers)
        {
            ValidateMethod(info);

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
