using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Registration;

namespace Unity
{
    public partial class ConstructorPipeline
    {
        #region Fields

        private static readonly ConstructorInfo PerResolveInfo = typeof(InternalPerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();

        protected static readonly Expression SetPerBuildSingletonExpr =
            Expression.Call(BuilderContextExpression.Context, 
                BuilderContextExpression.SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, BuilderContextExpression.Existing));

        protected static readonly Expression[] NoConstructorExpr = new [] {
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(StringFormat,
                            Expression.Constant("No public constructor is available for type {0}."),
                            BuilderContextExpression.Type))))};

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<Pipeline> enumerator,
                                                      Type type, ImplicitRegistration registration)
        {
            //// Select ConstructorInfo
            //var selector = GetOrDefault(registration);
            //var selection = selector.Select(type, registration)
            //                        .FirstOrDefault();

            //// Select constructor for the Type
            //object[]? resolvers = null;
            //ConstructorInfo? info = null;
            //IEnumerable<Expression> parametersExpr;

            //switch (selection)
            //{
            //    case ConstructorInfo memberInfo:
            //        info = memberInfo;
            //        parametersExpr = CreateParameterExpressions(info.GetParameters());
            //        break;

            //    case MethodBase<ConstructorInfo> injectionMember:
            //        info = injectionMember.MemberInfo(type);
            //        resolvers = injectionMember.Data;
            //        parametersExpr = CreateParameterExpressions(info.GetParameters(), resolvers);
            //        break;

            //    case Exception exception:
            //        return new[] {Expression.IfThen(
            //            Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
            //            Expression.Throw(Expression.Constant(exception)))};

            //    default:
            //        return NoConstructorExpr;
            //}

            //// Get lifetime manager
            //var lifetimeManager = (LifetimeManager?)registration.Get(typeof(LifetimeManager));

            //return lifetimeManager is PerResolveLifetimeManager
            //    ? new[] { GetResolverExpression(info, resolvers), SetPerBuildSingletonExpr }
            //    : new Expression[] { GetResolverExpression(info, resolvers) };

            yield break;
        }

        #endregion


        #region Overrides

        protected override Expression GetResolverExpression(ConstructorInfo info, object? resolvers)
        {
            var variable = Expression.Variable(info.DeclaringType);
            var parametersExpr = CreateParameterExpressions(info.GetParameters(), resolvers);

            return Expression.IfThen(
                Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                Expression.Block(new[] { variable }, new Expression[]
                {
                    Expression.Assign(variable, Expression.New(info, parametersExpr)),
                    Expression.Assign(BuilderContextExpression.Existing, Expression.Convert(variable, typeof(object)))
                }));
        }

        #endregion
    }
}
