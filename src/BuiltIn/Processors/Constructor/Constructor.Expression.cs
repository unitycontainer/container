using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region Fields

        //protected static readonly Expression[] NoConstructorExpr = new [] {
        //    Expression.IfThen(NullEqualExisting,
        //        Expression.Throw(
        //            Expression.New(InvalidRegistrationExpressionCtor,
        //                Expression.Call(StringFormat,
        //                    Expression.Constant("No public constructor is available for type {0}."),
        //                    PipelineContext.TypeExpression))))};

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder<IEnumerable<Expression>> builder)
        {
            var buildSteps = builder.Express();

            if (null != builder.Target) return buildSteps;

            // Select ConstructorInfo
            throw new NotImplementedException();
            // TODO:             var selection = Select(ref builder);

            //// Select constructor for the Type
            //ConstructorInfo info;
            //var expressions = Enumerable.Empty<Expression>();

            //switch (selection)
            //{
            //    case ConstructorInfo memberInfo:
            //        info = memberInfo;
            //        expressions = ParameterExpressions(info);
            //        break;

            //    case InjectionMethodBase<ConstructorInfo> injectionMember:
            //        info = injectionMember.MemberInfo(builder.Type);
            //        expressions = ParameterExpressions(info, injectionMember.Data);
            //        break;

            //    case Exception exception:
            //        return new[] {
            //            Expression.IfThen(NullEqualExisting, Expression.Throw(Expression.Constant(exception)))
            //        }.Concat(buildSteps);

            //    default:
            //        return NoConstructorExpr.Concat(buildSteps);
            //}

            //return builder.LifetimeManager is PerResolveLifetimeManager
            //    ? new[] { GetResolverExpression(info, expressions), SetPerBuildSingletonExpr }.Concat(buildSteps)
            //    : new Expression[] { GetResolverExpression(info, expressions) }.Concat(buildSteps);
        }

        #endregion


        #region Overrides

        protected override Expression GetResolverExpression(ConstructorInfo info, object? data)
        {
            throw new NotImplementedException();

            //Debug.Assert(null != data && data is IEnumerable<Expression>);
            //var parameters = (IEnumerable<Expression>)data!;

            //var variable = Expression.Variable(info.DeclaringType);

            //try
            //{
            //    return Expression.IfThen(
            //        Expression.Equal(Expression.Constant(null), PipelineContext.ExistingExpression),
            //        Expression.Block(new[] { variable }, new Expression[]
            //        {
            //            Expression.Assign(variable, Expression.New(info, parameters)),
            //            Expression.Assign(PipelineContext.ExistingExpression, Expression.Convert(variable, typeof(object)))
            //        }));
            //}
            //catch (InvalidRegistrationException reg)
            //{
            //    return Expression.IfThen(NullEqualExisting, Expression.Throw(Expression.Constant(reg)));
            //}
            //catch (Exception ex)
            //{
            //    return Expression.IfThen(NullEqualExisting, Expression.Throw(Expression.Constant(new InvalidRegistrationException(ex.Message, ex))));
            //}
        }

        #endregion
    }
}
