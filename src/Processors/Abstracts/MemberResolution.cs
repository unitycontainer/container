using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public delegate ResolveDelegate<BuilderContext> ResolutionAttributeFactory(Attribute attribute, object info, object resolver, object defaultValue);

    public abstract partial class MemberProcessor<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Member Processing

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


        protected virtual ResolveDelegate<BuilderContext> ResolverFromMemberInfo(TMemberInfo info)
        {
            foreach (var node in AttributeFactories)
            {
                var attribute = GetCustomAttribute(info, node.Type);
                if (null == attribute || null == node.ResolutionFactory)
                    continue;

                return node.ResolutionFactory(attribute, info, DependencyAttribute.Instance, null);
            }

            return GetResolverDelegate(info, DependencyAttribute.Instance);
        }

        protected virtual ResolveDelegate<BuilderContext> ResolverFromMemberInfo(TMemberInfo info, TData resolver)
        {
            return GetResolverDelegate(info, resolver);
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object resolver)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Parameter Resolver Factories

        protected abstract ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null);

        protected abstract ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null);

        #endregion
    }
}
