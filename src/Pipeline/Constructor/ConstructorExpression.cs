using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity
{
    public partial class ConstructorPipeline
    {
        #region Fields

        private static readonly ConstructorInfo PerResolveInfo = typeof(RuntimePerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();

        protected static readonly Expression SetPerBuildSingletonExpr =
            Expression.Call(PipelineContextExpression.Context, 
                PipelineContextExpression.SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, PipelineContextExpression.Existing));

        protected static readonly Expression[] NoConstructorExpr = new [] {
            Expression.IfThen(NullTestExpression,
                Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(StringFormat,
                            Expression.Constant("No public constructor is available for type {0}."),
                            PipelineContextExpression.Type))))};

        public object BuilderContextExpression { get; private set; }

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
                    return new[] {Expression.IfThen(
                        Expression.Equal(Expression.Constant(null), PipelineContextExpression.Existing),
                        Expression.Throw(Expression.Constant(exception)))};

                default:
                    return NoConstructorExpr;
            }

            // Get lifetime manager
            var lifetimeManager = (LifetimeManager?)builder.Policies?.Get(typeof(LifetimeManager));

            return lifetimeManager is PerResolveLifetimeManager
                ? GetConstructorExpression(info, resolvers).Concat(new[] { SetPerBuildSingletonExpr })
                                                           .Concat(expressions)
                : GetConstructorExpression(info, resolvers).Concat(expressions);
        }

        #endregion


        #region Overrides

        protected virtual IEnumerable<Expression> GetConstructorExpression(ConstructorInfo info, object? resolvers)
        {
            var parametersExpr = CreateParameterExpressions(info.GetParameters(), resolvers);

            yield return Expression.IfThen(NullTestExpression,
                Expression.Assign(PipelineContextExpression.Existing, 
                Expression.Convert(Expression.New(info, parametersExpr), typeof(object))));
        }


        #endregion
    }
}
