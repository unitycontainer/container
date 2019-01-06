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
            ValidateMember(info);

            var parameterResolvers = CreateParameterResolvers(info.GetParameters()).ToArray();
            return GetResolverDelegate(info, parameterResolvers);
        }

        protected override ResolveDelegate<BuilderContext> ResolverFromMemberInfo(MethodInfo info, object[] resolvers)
        {
            ValidateMember(info);

            var parameterResolvers = CreateParameterResolvers(info.GetParameters(), resolvers).ToArray();
            return GetResolverDelegate(info, parameterResolvers);
        }

        #endregion
    }
}
