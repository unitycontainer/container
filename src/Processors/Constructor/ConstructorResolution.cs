using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public partial class ConstructorProcessor 
    {
        #region Overrides

        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext>? seed)
        {
            // Select ConstructorInfo
            var selection = SelectConstructor(type, registration);

            // Select constructor for the Type
            ConstructorInfo info;
            object[]? resolvers = null;

            switch (selection)
            {
                case ConstructorInfo memberInfo:
                    info = memberInfo;
                    resolvers = CreateParameterResolvers(info);
                    break;

                case MethodBase<ConstructorInfo> injectionMember:
                    info = injectionMember.MemberInfo(type);
                    resolvers = null != injectionMember.Data && injectionMember.Data is object[] injectors && 0 != injectors.Length
                              ? CreateParameterResolvers(info, injectors)
                              : CreateParameterResolvers(info);
                    break;

                case Exception exception:
                    return (ref BuilderContext c) =>
                    {
                        if (null == c.Existing)
                            throw exception;

                        return c.Existing;
                    };

                default:
                    return (ref BuilderContext c) =>
                    {
                        if (null == c.Existing)
                            throw new InvalidOperationException($"No public constructor is available for type {c.Type}.",
                                new InvalidRegistrationException());

                        return c.Existing;
                    };
            }

            // Get lifetime manager
            var lifetimeManager = (LifetimeManager?)registration.Get(typeof(LifetimeManager));

            return lifetimeManager is PerResolveLifetimeManager
                ? GetPerResolveDelegate(info, resolvers)
                : GetResolverDelegate(info, resolvers);
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(ConstructorInfo info, object? data)
        {
            Debug.Assert(null != data && data is ResolveDelegate<BuilderContext>[]);
            var resolvers = (ResolveDelegate<BuilderContext>[])data!;

            return (ref BuilderContext c) =>
            {
                if (null != c.Existing) return c.Existing;

                var dependencies = new object?[resolvers.Length];
                for (var i = 0; i < dependencies.Length; i++) dependencies[i] = resolvers[i](ref c);

                c.Existing = info.Invoke(dependencies);

                return c.Existing;
            };
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<BuilderContext> GetPerResolveDelegate(ConstructorInfo info, object? data)
        {
            Debug.Assert(null != data && data is ResolveDelegate<BuilderContext>[]);
            var resolvers = (ResolveDelegate<BuilderContext>[])data!;

            // PerResolve lifetime
            return (ref BuilderContext c) =>
            {
                if (null != c.Existing) return c.Existing;

                var dependencies = new object?[resolvers.Length];
                for (var i = 0; i < dependencies.Length; i++)
                    dependencies[i] = resolvers[i](ref c);

                c.Existing = info.Invoke(dependencies);
                c.Set(typeof(LifetimeManager), new InternalPerResolveLifetimeManager(c.Existing));

                return c.Existing;
            };
        }

        #endregion
    }
}
