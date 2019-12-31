using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class ConstructorProcessor 
    {
        #region Fields

        protected static readonly ConstructorInfo InvalidOperationExceptionCtor =
            typeof(InvalidOperationException)
                .GetTypeInfo()
                .DeclaredConstructors
                .First(c =>
                {
                    var parameters = c.GetParameters();
                    return 2 == parameters.Length &&
                           typeof(string) == parameters[0].ParameterType &&
                           typeof(Exception) == parameters[1].ParameterType;
                });

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
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(StringFormat,
                            Expression.Constant("No public constructor is available for type {0}."),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)))};

        #endregion


        #region Overrides

        public override IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration)
        {
            // Select ConstructorInfo
            var selection = SelectConstructor(type, registration);

            // Select constructor for the Type
            var expressions = Enumerable.Empty<Expression>();

            ConstructorInfo info;
            switch (selection)
            {
                case ConstructorInfo memberInfo:
                    info = memberInfo;
                    expressions = CreateParameterExpressions(info);
                    break;

                case MethodBase<ConstructorInfo> injectionMember:
                    info = injectionMember.MemberInfo(type);
                    expressions = CreateParameterExpressions(info, injectionMember.Data);
                    break;

                case Exception exception:
                    return new[] {Expression.IfThen(
                        Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                        Expression.Throw(Expression.Constant(exception)))};

                default:
                    return NoConstructorExpr;
            }

            // Get lifetime manager
            var lifetimeManager = (LifetimeManager?)registration.Get(typeof(LifetimeManager));

            return lifetimeManager is PerResolveLifetimeManager
                ? new[] { GetResolverExpression(info, expressions), SetPerBuildSingletonExpr }
                : new Expression[] { GetResolverExpression(info, expressions) };
        }

        protected override Expression GetResolverExpression(ConstructorInfo info, object? data)
        {
            Debug.Assert(null != data && data is IEnumerable<Expression>);
            var resolvers = (IEnumerable<Expression>)data!;

            var variable = Expression.Variable(info.DeclaringType);

            try
            {
                return Expression.IfThen(
                    Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                    Expression.Block(new[] { variable }, new Expression[]
                    {
                        Expression.Assign(variable, Expression.New(info, resolvers)),
                        Expression.Assign(BuilderContextExpression.Existing, Expression.Convert(variable, typeof(object)))
                    }));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidRegistrationException("Invalid Argument", ex);
            }
        }

        #endregion
    }
}
