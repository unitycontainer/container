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
            return GetAttributeResolver(info, DependencyAttribute.Instance) ?? 
                   GetResolverDelegate(info, DependencyAttribute.Instance);
        }

        TData GetParameterResolver(TMemberInfo info, object data)
        {
            throw new NotImplementedException();
        }

        protected virtual ResolveDelegate<BuilderContext> ResolverFromMemberInfo(TMemberInfo info, TData resolver)
        {
            return GetResolverDelegate(info, resolver);
        }

        #endregion


        #region Implementation


        protected virtual ResolveDelegate<BuilderContext> GetDataResolver(TMemberInfo info, object resolver)
        {
            switch (resolver)
            {
                case DependencyAttribute dependencyAttribute:
                    return (ref BuilderContext context) => context.Resolve(MemberType(info), dependencyAttribute.Name);

                case OptionalDependencyAttribute optionalAttribute:
                    return (ref BuilderContext context) => 
                    {
                        try { return context.Resolve(MemberType(info), optionalAttribute.Name); }
                        catch { return null; }
                    };

                case IResolve policy:
                    return policy.Resolve;

                case IResolverFactory<TMemberInfo> memberFactory:
                    return memberFactory.GetResolver<BuilderContext>(info);

                case IResolverFactory typeFactory:
                    return typeFactory.GetResolver<BuilderContext>(MemberType(info));

                case Type type:
                    return (ref BuilderContext context) => typeof(Type) == MemberType(info) 
                    ? type : context.Resolve(type, null);
            }

            return (ref BuilderContext context) => resolver;
        }


        private ResolveDelegate<BuilderContext> GetAttributeResolver(TMemberInfo info, object resolver)
        {
            foreach (var node in AttributeFactories)
            {
                var attribute = GetCustomAttribute(info, node.Type);
                if (null == attribute || null == node.ResolutionFactory)
                    continue;

                return node.ResolutionFactory(attribute, info, resolver, null);
            }

            return null;
        }


        protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object resolver)
        {
            switch(resolver)
            {
                case DependencyAttribute dependencyAttribute 
                when ReferenceEquals(dependencyAttribute, DependencyAttribute.Instance):
                    break;

                case OptionalDependencyAttribute optionalAttribute
                when ReferenceEquals(optionalAttribute, OptionalDependencyAttribute.Instance):
                    break;
            }

            throw new NotImplementedException();
        }

        #endregion


        #region Parameter Resolver Factories

        // Default expression factory for [Dependency] attribute
        protected abstract ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null);

        // Default expression factory for [OptionalDependency] attribute
        protected abstract ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null);

        #endregion
    }
}
