using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : MethodBaseInfoProcessor<ConstructorInfo>
    {
        #region Overrides

        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed)
        {
            var selector = GetPolicy<ISelect<ConstructorInfo>>(registration);
            var selection = selector.Select(type, registration)
                                    .FirstOrDefault();
            
            // Validate constructor info
            if (null == selection)
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException($"No public constructor is available for type {c.Type}.", 
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            return ValidateConstructedTypeResolver(type) ?? 
                   BuildResolver(registration, type, selection);
        }

        #endregion


        #region Implementation

        private ResolveDelegate<BuilderContext> BuildResolver(IPolicySet registration, Type type, object selection)
        {
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
                    throw new InvalidOperationException($"Unknown constructor type");
            }

            var parameterResolvers = CreateParameterResolvers(info.GetParameters(), resolvers).ToArray();

            if (registration.Get(typeof(LifetimeManager)) is PerResolveLifetimeManager)
            {
                // PerResolve lifetime
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                    {
                        var parameters = new object[parameterResolvers.Length];
                        for (var i = 0; i < parameters.Length; i++)
                            parameters[i] = parameterResolvers[i](ref c);

                        c.Existing = info.Invoke(parameters);
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
                    var parameters = new object[parameterResolvers.Length];
                    for (var i = 0; i < parameters.Length; i++)
                        parameters[i] = parameterResolvers[i](ref c);

                    c.Existing = info.Invoke(parameters);
                }

                return c.Existing;
            };
        }

        private ResolveDelegate<BuilderContext> ValidateConstructedTypeResolver(Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsInterface)
#else
            if (type.IsInterface)
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructInterface, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsAbstract)
#else
            if (type.IsAbstract)
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructAbstractClass, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsSubclassOf(typeof(Delegate)))
#else
            if (type.IsSubclassOf(typeof(Delegate)))
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructDelegate, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            if (type == typeof(string))
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.TypeIsNotConstructable, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            return null;
        }

        #endregion
    }
}
