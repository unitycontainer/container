using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public delegate ResolveDelegate<BuilderContext> MemberResolverFactory(Attribute attribute, object info, object resolver, object defaultValue);

    public abstract partial class BuildMemberProcessor<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Fields

        protected (Type type, MemberResolverFactory factory)[] ResolverFactories;

        #endregion


        #region Public Methods

        public void Add(Type type, MemberResolverFactory factory)
        {
            for (var i = 0; i < ResolverFactories.Length; i++)
            {
                if (ResolverFactories[i].type != type) continue;
                ResolverFactories[i].factory = factory;
                return;
            }

            var factories = new (Type type, MemberResolverFactory factory)[ResolverFactories.Length + 1];
            Array.Copy(ResolverFactories, factories, ResolverFactories.Length);
            factories[ResolverFactories.Length] = (type, factory);
            ResolverFactories = factories;
        }

        #endregion


        #region Overrides

        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed)
        {
            var selector = GetPolicy<ISelect<TMemberInfo>>(registration);
            var members = selector.Select(type, registration);
            var resolvers = ResolversFromSelected(type, members).ToArray();

            return (ref BuilderContext c) =>
            {
                if (null == (c.Existing = seed(ref c))) return null;

                foreach (var resolver in resolvers) resolver(ref c);

                return c.Existing;
            };
        }

        #endregion


        #region Build Resolver 

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> ResolversFromSelected(Type type, IEnumerable<object> members)
        {
            foreach (var member in members)
            {

                switch (member)
                {
                    case TMemberInfo memberInfo:
                        yield return BuildMemberResolver(memberInfo, default);
                        break;

                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        var (info, value) = injectionMember.FromType(type);
                        yield return BuildMemberResolver(info, value);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        protected virtual ResolveDelegate<BuilderContext> BuildMemberResolver(TMemberInfo info, TData resolver)
        {
            foreach (var pair in ResolverFactories)
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                var attribute = info.GetCustomAttributes()
                                    .Where(a => a.GetType()
                                                 .GetTypeInfo()
                                                 .IsAssignableFrom(pair.type.GetTypeInfo()))
                                    .FirstOrDefault();
#else
                var attribute = info.GetCustomAttribute(pair.type);
#endif
                if (null == attribute || null == pair.factory)
                    continue;

                return pair.factory(attribute, info, resolver, null);
            }

            return GetResolver(info, resolver);
        }

        protected virtual ResolveDelegate<BuilderContext> GetResolver(TMemberInfo info, object resolver) => throw new NotImplementedException();

        #endregion


        #region Parameter Resolver Factories

        // Default expression factory for [Dependency] attribute
        protected abstract ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null);

        // Default expression factory for [OptionalDependency] attribute
        protected abstract ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null);

        #endregion
    }
}
