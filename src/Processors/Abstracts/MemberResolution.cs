using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public delegate ResolveDelegate<BuilderContext> ResolutionAttributeFactory<TMemberInfo>(Attribute attribute, TMemberInfo info)
        where TMemberInfo : MemberInfo;

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

                var factory = (ResolutionAttributeFactory<TMemberInfo>)node.ResolutionFactory;

                return GetResolverDelegate(info, factory(attribute, info));
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

        protected virtual ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, TMemberInfo info)
        {
            var type = MemberType(info);
            return (ref BuilderContext context) => context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected virtual ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, TMemberInfo info)
        {
            var type = MemberType(info);
            return (ref BuilderContext context) =>
            {
                try { return context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name); }
                catch { return null; }
            };
        }

        #endregion
    }
}
