using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class MethodBaseProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Fields

        private readonly MethodInfo ResolveParameter =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        #endregion

        
        #region Constructors

        protected MethodBaseProcessor(IPolicySet policySet, Type attribute)
            : base(policySet)
        {
            Add(attribute, (ExpressionAttributeFactory)null, (ResolutionAttributeFactory)null);
        }

        #endregion


        #region Overrides

        protected override Type MemberType(TMemberInfo info) => info.DeclaringType;

        #endregion


        #region Implementation

        private object PreProcessResolver(ParameterInfo parameter, object resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<BuilderContext>)policy.Resolve;

                case IResolverFactory<ParameterInfo> factory:
                    return factory.GetResolver<BuilderContext>(parameter);

                case Type type:
                    return typeof(Type) == parameter.ParameterType
                        ? type : (object)parameter;
            }

            return resolver;
        }

        #endregion
    }
}
