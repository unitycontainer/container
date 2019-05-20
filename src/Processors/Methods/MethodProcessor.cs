using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public class MethodProcessor : ParametersProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(IPolicySet policySet, UnityContainer container)
            : base(policySet, typeof(InjectionMethodAttribute), container)
        {
        }

        #endregion


        #region Selection

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
            return type.GetDeclaredMethods()
                       .Where(member => !member.IsFamily && 
                                        !member.IsPrivate && 
                                        !member.IsStatic);
        }

        #endregion

        #region Expression 

        protected override Expression GetResolverExpression(MethodInfo info, object resolvers)
        {
            try
            {
                return Expression.Call(
                    Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                    info, CreateParameterExpressions(info.GetParameters(), resolvers));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidRegistrationException("Invalid Argument", ex);
            }
        }

        #endregion


        #region Resolution

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
