using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Selection Processing

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
                        yield return ExpressionFromMemberInfo(injectionMember.MemberInfo(type), 
                                                              injectionMember.Data);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        #endregion


        #region Members Processing

        protected virtual Expression ExpressionFromMemberInfo(TMemberInfo info)
        {
            var member = CreateMemberExpression(info);

            foreach (var node in AttributeFactories)
            {
                var attribute = GetCustomAttribute(info, node.Type);
                if (null == attribute) continue;

                return null == node.Factory
                    ? Expression.Assign(member, GetResolverExpression(info, null, attribute))
                    : Expression.Assign(member, GetResolverExpression(info, null, node.Factory(attribute, info)));
            }

            return Expression.Assign(member, GetResolverExpression(info, null, DependencyAttribute.Instance));
        }

        protected virtual Expression ExpressionFromMemberInfo(TMemberInfo info, TData resolver)
        {
            return Expression.Assign(CreateMemberExpression(info), GetResolverExpression(info, null, resolver));
        }

        #endregion


        #region Implementation

        protected virtual Expression GetResolverExpression(TMemberInfo info, string name, object resolver) 
            => throw new NotImplementedException();

        protected virtual MemberExpression CreateMemberExpression(TMemberInfo info) 
            => throw new NotImplementedException();

        #endregion
    }
}
