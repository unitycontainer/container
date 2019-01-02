using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class ConstructorProcessor 
    {
        #region Overrides

        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed)
        {
            // Select ConstructorInfo
            var selector = GetPolicy<ISelect<ConstructorInfo>>(registration);
            var selection = selector.Select(type, registration)
                                    .FirstOrDefault();

            // Select appropriate ctor for the Type
            ConstructorInfo info;
            object[] resolvers = null;

            switch (selection)
            {
                case ConstructorInfo memberInfo:
                    info = memberInfo;
                    break;

                case MethodBaseMember<ConstructorInfo> injectionMember:
                    (info, resolvers) = injectionMember.FromType(type);
                    break;

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
            var lifetimeManager = (LifetimeManager)registration.Get(typeof(LifetimeManager));

            // Create dependency resolvers
            var parameterResolvers = CreateParameterResolvers(info.GetParameters(), resolvers).ToArray();
            if (lifetimeManager is PerResolveLifetimeManager)
            {
                // PerResolve lifetime
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                    {
                        var dependencies = new object[parameterResolvers.Length];
                        for (var i = 0; i < dependencies.Length; i++)
                            dependencies[i] = parameterResolvers[i](ref c);

                        c.Existing = info.Invoke(dependencies);
                        c.Set(typeof(LifetimeManager),
                              new InternalPerResolveLifetimeManager(c.Existing));
                    }

                    return c.Existing;
                };
            }

            // Normal activator
            return (ref BuilderContext c) =>
            {
                if (null == c.Existing)
                {
                    var dependencies = new object[parameterResolvers.Length];
                    for (var i = 0; i < dependencies.Length; i++)
                        dependencies[i] = parameterResolvers[i](ref c);

                    c.Existing = info.Invoke(dependencies);
                }

                return c.Existing;
            };

        }

        #endregion
    }
}
