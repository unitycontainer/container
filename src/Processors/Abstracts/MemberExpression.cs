using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public delegate Expression ExpressionAttributeFactory(Attribute attribute, Expression member, object info, Type type, object resolver);

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

                return node.ExpressionFactory(attribute, member, info, MemberType(info), null);
            }

            return Expression.Assign(member, GetResolverExpression(info, null, null));
        }

        protected virtual Expression ExpressionFromMemberInfo(TMemberInfo info, TData resolver)
        {
            return Expression.Assign(CreateMemberExpression(info), GetResolverExpression(info, null, resolver));
        }

        #endregion


        #region Implementation


        private Expression ExpressionFromAttribute(TMemberInfo info, object resolver)
        {
            var member = CreateMemberExpression(info);

            foreach (var node in AttributeFactories)
            {
                var attribute = GetCustomAttribute(info, node.Type);
                if (null == attribute || null == node.ExpressionFactory)
                    continue;

                return node.ExpressionFactory(attribute, member, info, MemberType(info), null);
            }

            return null;
        }


        protected virtual Expression GetResolverExpression(TMemberInfo info, string name, object resolver) => throw new NotImplementedException();

        protected virtual MemberExpression CreateMemberExpression(TMemberInfo info) => throw new NotImplementedException();

        #endregion


        #region Parameter Expression Factories

        protected virtual Expression DependencyExpressionFactory(Attribute attribute, Expression member, object memberInfo, Type type, object resolver)
        {
            TMemberInfo info = (TMemberInfo)memberInfo;
            return Expression.Assign(member, GetResolverExpression(info, ((DependencyResolutionAttribute)attribute).Name, resolver ?? DependencyAttribute.Instance));
        }

        protected virtual Expression OptionalDependencyExpressionFactory(Attribute attribute, Expression member, object memberInfo, Type type, object resolver)
        {
            TMemberInfo info = (TMemberInfo)memberInfo;
            return Expression.TryCatch(
                        Expression.Assign(member, GetResolverExpression(info, ((OptionalDependencyAttribute)attribute).Name, resolver ?? OptionalDependencyAttribute.Instance)),
                    Expression.Catch(typeof(Exception),
                        Expression.Assign(member, Expression.Constant(null, type))));
        }

#endregion
    }
}
