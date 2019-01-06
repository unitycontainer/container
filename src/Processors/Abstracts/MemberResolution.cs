using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Selection Processing

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> ResolversFromSelection(Type type, IEnumerable<object> members)
        {
            foreach (var member in members)
            {
                switch (member)
                {
                    case TMemberInfo memberInfo:
                        yield return ResolverFromMemberInfo(memberInfo);
                        break;

                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        var (info, value) = injectionMember.FromType(type);
                        yield return ResolverFromMemberInfo(info, value);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        #endregion


        #region Member Processing

        protected virtual ResolveDelegate<BuilderContext> ResolverFromMemberInfo(TMemberInfo info)
        {
            ValidateMember(info);

            foreach (var node in AttributeFactories)
            {
                var attribute = GetCustomAttribute(info, node.Type);
                if (null == attribute) continue;

                return null == node.Factory
                    ? GetResolverDelegate(info, attribute)
                    : GetResolverDelegate(info, node.Factory(attribute, info)); 
            }

            return GetResolverDelegate(info, DependencyAttribute.Instance);
        }

        protected virtual ResolveDelegate<BuilderContext> ResolverFromMemberInfo(TMemberInfo info, TData resolver)
        {
            ValidateMember(info);
            return GetResolverDelegate(info, resolver);
        }

        #endregion


        #region Implementation

        protected abstract ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object resolver);

        #endregion
    }
}
