using System;
using System.Linq.Expressions;
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

        protected override Expression ExpressionFromMemberInfo(MethodInfo info)
        {
            ValidateMember(info);

            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = Expression.Block(typeof(void),
                Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(info, typeof(object))),
                Expression.Rethrow(typeof(void)));

            return Expression.TryCatch(base.ExpressionFromMemberInfo(info), 
                   Expression.Catch(ex, block));
        }

        protected override Expression ExpressionFromMemberInfo(MethodInfo info, object[] resolvers)
        {
            ValidateMember(info);

            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = Expression.Block(typeof(void),
                Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(info, typeof(object))),
                Expression.Rethrow(typeof(void)));

            return Expression.TryCatch(base.ExpressionFromMemberInfo(info, resolvers),
                   Expression.Catch(ex, block));
        }

        protected override Expression CreateParameterExpression(ParameterInfo parameter, object resolver)
        {
            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = Expression.Block(parameter.ParameterType,
                Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(parameter, typeof(object))),
                Expression.Rethrow(parameter.ParameterType));

            return Expression.TryCatch(base.CreateParameterExpression(parameter, resolver),
                   Expression.Catch(ex, block));
        }

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
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }

                return c.Existing;
            };
        }

        protected override ResolveDelegate<BuilderContext> CreateParameterResolver(ParameterInfo parameter, object resolver)
        {
            var resolverDelegate = base.CreateParameterResolver(parameter, resolver);

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
