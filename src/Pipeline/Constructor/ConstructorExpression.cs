using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity
{
    public partial class ConstructorPipeline
    {
        #region Fields

        protected static readonly Expression[] NoConstructorExpr = new [] {
            Expression.IfThen(NullEqualExisting,
                Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(StringFormat,
                            Expression.Constant("No public constructor is available for type {0}."),
                            PipelineContextExpression.Type))))};

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            var expressions = builder.Express();

            if (null != builder.SeedExpression) return expressions;

            // Select ConstructorInfo
            var selector = GetOrDefault(builder.Policies);
            var selection = selector.Invoke(builder.Type, builder.InjectionMembers)
                                    .FirstOrDefault();

            // Select constructor for the Type
            ConstructorInfo info;
            object[]? resolvers = null;

            switch (selection)
            {
                case ConstructorInfo memberInfo:
                    info = memberInfo;
                    break;

                case MethodBase<ConstructorInfo> injectionMember:
                    info = injectionMember.MemberInfo(builder.Type);
                    resolvers = injectionMember.Data;
                    break;

                case Exception exception:
                    return new[] {
                        Expression.IfThen(NullEqualExisting, Expression.Throw(Expression.Constant(exception)))
                    }.Concat(expressions);

                default:
                    return NoConstructorExpr.Concat(expressions);
            }

            return new[] { GetResolverExpression(info, resolvers, builder.LifetimeManager is PerResolveLifetimeManager) }.Concat(expressions);
        }

        #endregion


        #region Overrides

                //? new[] { GetResolverExpression(info, resolvers),  }.Concat(expressions)
        protected virtual Expression GetResolverExpression(ConstructorInfo info, object? resolvers, bool perResolve)
        {
            try
            {
                var parameters = info.GetParameters();
                var variables = VariableExpressions(parameters);

                if (perResolve)
                    return Expression.IfThen(NullEqualExisting,
                        Expression.Block(variables, ParameterExpressions(variables, parameters, resolvers)
                                                   .Concat(new[] {
                                                           Expression.Assign(
                                                               PipelineContextExpression.Existing,
                                                               Expression.Convert(
                                                                   Expression.New(info, variables), typeof(object)))})
                                                   .Concat(new[] { SetPerBuildSingletonExpr })));
                else
                    return Expression.IfThen(NullEqualExisting,
                        Expression.Block(variables, ParameterExpressions(variables, parameters, resolvers)
                                                   .Concat(new[] {
                                                           Expression.Assign(
                                                               PipelineContextExpression.Existing,
                                                               Expression.Convert(
                                                                   Expression.New(info, variables), typeof(object)))})));
            }
            catch (InvalidRegistrationException reg)
            {
                return Expression.IfThen(NullEqualExisting, Expression.Throw(Expression.Constant(reg)));
            }
            catch (Exception ex)
            {
                return Expression.IfThen(NullEqualExisting, Expression.Throw(Expression.Constant(new InvalidRegistrationException(ex))));
            }
        }

        #endregion
    }
}
