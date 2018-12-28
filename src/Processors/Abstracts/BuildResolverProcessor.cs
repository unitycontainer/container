using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public delegate ResolveDelegate<BuilderContext> MemberResolverFactory(Attribute attribute, object info, string name, object resolver, object defaultValue);

    public abstract partial class BuildMemberProcessor<TMemberInfo, TData> : BuildMemberProcessor
                                                         where TMemberInfo : MemberInfo
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

        public override ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context, ResolveDelegate<BuilderContext> seed)
        {
            var selector = GetPolicy<ISelect<TMemberInfo>>(ref context, context.RegistrationType, context.RegistrationName);
            var members = selector.Select(ref context);
            var resolvers = ResolversFromSelected(context.Type, context.Name, members);

            return (ref BuilderContext c) =>
            {
                // Constructor could be overridden 
                if (null != seed) c.Existing = seed(ref c);

                foreach (var resolver in resolvers)
                    c.Existing = resolver(ref c);

                return c.Existing;
            };
        }

        #endregion


        #region Build Resolver 

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> ResolversFromSelected(Type type, string name, IEnumerable<object> members)
        {
            foreach (var member in members)
            {

                switch (member)
                {
                    case TMemberInfo memberInfo:
                        yield return BuildMemberResolver(memberInfo, name, default);
                        break;

                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        var (info, value) = injectionMember.FromType(type);
                        yield return BuildMemberResolver(info, name, value);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        protected virtual ResolveDelegate<BuilderContext> BuildMemberResolver(TMemberInfo info, string name, TData resolver)
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

                return pair.factory(attribute, info, name, resolver, null);
            }

            return GetResolver(info, name, resolver);
        }

        protected virtual ResolveDelegate<BuilderContext> GetResolver(TMemberInfo info, string name, object resolver) => throw new NotImplementedException();

        #endregion


        #region Parameter Resolver Factories

        // Default expression factory for [Dependency] attribute
        protected abstract ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, string name, object resolver, object defaultValue = null);

        // Default expression factory for [OptionalDependency] attribute
        protected abstract ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, string name, object resolver, object defaultValue = null);

        #endregion
    }
}
