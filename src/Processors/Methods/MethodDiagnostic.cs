using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

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

        protected override Expression GetResolverExpression(MethodInfo info, object resolvers)
        {
            ValidateMember(info);

            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = Expression.Block(typeof(void),
                Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(info, typeof(object))),
                Expression.Rethrow(typeof(void)));

            return 
                Expression.TryCatch(
                    Expression.Call(
                        Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                        info, CreateDiagnosticParameterExpressions(info.GetParameters(), resolvers)),
                Expression.Catch(ex, block));
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object resolvers)
        {
            ValidateMember(info);

            var parameterResolvers = CreateDiagnosticParameterResolvers(info.GetParameters(), resolvers).ToArray();
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
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }

                return c.Existing;
            };
        }

        #endregion
    }
}
