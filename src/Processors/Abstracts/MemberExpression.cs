using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Processors
{
    public delegate Expression ExpressionAttributeFactory<TMemberInfo>(Attribute attribute, Expression member, TMemberInfo info, Type type, object resolver)
        where TMemberInfo : MemberInfo;

    public abstract partial class MemberProcessor<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Members Processing

        protected virtual IEnumerable<Expression> ExpressionsFromSelection(Type type, IEnumerable<object> members)
        {
            foreach (var member in members)
            {

                switch (member)
                {
                    case TMemberInfo memberInfo:
                        yield return ExpressionFromMemberInfo(memberInfo);
                        break;

                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        var (info, value) = injectionMember.FromType(type);
                        yield return ExpressionFromMemberInfo(info, value);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        protected virtual Expression ExpressionFromMemberInfo(TMemberInfo info)
        {
            var member = CreateMemberExpression(info);

            foreach (var node in AttributeFactories)
            {
                var attribute = GetCustomAttribute(info, node.Type);
                if (null == attribute || null == node.ExpressionFactory)
                    continue;

                var factory = (ExpressionAttributeFactory<TMemberInfo>)node.ExpressionFactory;

                return Expression.Assign(member, GetResolverExpression(info, null, factory(attribute, member, info, MemberType(info), null)));
            }

            return Expression.Assign(member, GetResolverExpression(info, null, null));
        }

        protected virtual Expression ExpressionFromMemberInfo(TMemberInfo info, TData resolver)
        {
            return Expression.Assign(CreateMemberExpression(info), GetResolverExpression(info, null, resolver));
        }

        #endregion


        #region Implementation

        protected virtual Expression GetResolverExpression(TMemberInfo info, string name, Expression resolver) => throw new NotImplementedException();

        protected virtual Expression GetResolverExpression(TMemberInfo info, string name, object resolver) => throw new NotImplementedException();

        protected virtual MemberExpression CreateMemberExpression(TMemberInfo info) => throw new NotImplementedException();

        #endregion


        #region Parameter Expression Factories

        protected virtual Expression DependencyExpressionFactory(Attribute attribute, Expression member, TMemberInfo info, Type type, object resolver)
        {
            var dependencyType = MemberType(info);
            var resolve = Expression.Call(
                            BuilderContextExpression.Context,
                            BuilderContextExpression.ResolveMethod,
                            Expression.Constant(dependencyType, typeof(Type)),
                            Expression.Constant(((DependencyResolutionAttribute)attribute).Name, typeof(string)));

            return Expression.Convert(resolve, dependencyType);
        }

        protected virtual Expression OptionalDependencyExpressionFactory(Attribute attribute, Expression member, TMemberInfo info, Type type, object resolver)
        {
            var dependencyType = MemberType(info);
            var variable = Expression.Variable(dependencyType);
            var resolve = Expression.Call(
                            BuilderContextExpression.Context,
                            BuilderContextExpression.ResolveMethod,
                            Expression.Constant(dependencyType, typeof(Type)),
                            Expression.Constant(((DependencyResolutionAttribute)attribute).Name, typeof(string)));

            var expression = Expression.Block(new[] { variable }, new Expression[]
            {
                Expression.TryCatch(
                    Expression.Assign(variable, Expression.Convert(resolve, dependencyType)),
                Expression.Catch(typeof(Exception),
                    Expression.Assign(variable, Expression.Constant(null, dependencyType)))),
                variable
            });

            return expression;
        }

        #endregion
    }
}
