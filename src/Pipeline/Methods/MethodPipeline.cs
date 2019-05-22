using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity
{
    public partial class MethodPipeline : ParametersPipeline<MethodInfo>
    {
        #region Constructors

        public MethodPipeline(UnityContainer container)
            : base(typeof(InjectionMethodAttribute), container)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
            return type.GetDeclaredMethods()
                       .Where(member => !member.IsFamily && 
                                        !member.IsPrivate && 
                                        !member.IsStatic);
        }

        public override MemberSelector<MethodInfo> GetOrDefault(IPolicySet? registration) => 
            registration?.Get<MemberSelector<MethodInfo>>() ?? Defaults.SelectMethod;

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(MethodInfo info, object? resolvers)
        {
            return Expression.Call(
                Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                info, CreateParameterExpressions(info.GetParameters(), resolvers));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object? resolvers)
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
