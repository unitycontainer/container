using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : MethodBaseInfoProcessor<ConstructorInfo>
    {
        #region Overrides

        public override ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context, ResolveDelegate<BuilderContext> seed)
        {
            // Verify the type we're trying to build is actually constructable -
            // CLR primitive types like string and int aren't.
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (!context.Type.GetTypeInfo().IsInterface)
#else
            if (!context.Type.IsInterface)
#endif
            {
                if (context.Type == typeof(string))
                {
                    throw new InvalidOperationException(
                        $"The type {context.Type.Name} cannot be constructed. You must configure the container to supply this value.");
                }
            }

            var selector = GetPolicy<ISelect<ConstructorInfo>>(ref context, context.RegistrationType, context.RegistrationName);
            var selection = selector.Select(ref context)
                               .FirstOrDefault();
            
            // Validate constructor info
            if (null == selection)
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException($"No public constructor is available for type {c.Type}.");

                    return c.Existing;
                };
            }

            return ValidateConstructedTypeResolver(ref context) ?? BuildResolver(context.Type, selection);

            //var resolvers = GetResolvers(context.Type, context.Name, members);

            //var IfThenExpr = Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
            //        ValidateConstructedType(ref context) ?? newExpr);

            //return context.Registration.Get(typeof(LifetimeManager)) is PerResolveLifetimeManager
            //    ? new[] { IfThenExpr, BuilderContextExpression.Set(context.RegistrationType,
            //                                                       context.RegistrationName,
            //                                                       typeof(LifetimeManager),
            //                                                       Expression.New(PerResolveInfo,
            //                                                                      BuilderContextExpression.Existing)) }
            //    : new Expression[] { IfThenExpr };

            //return base.GetResolver(ref context, seed);
        }


        #endregion


        #region Implementation

        private ResolveDelegate<BuilderContext> BuildResolver(Type type, object selection)
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

        private ResolveDelegate<BuilderContext> ValidateConstructedTypeResolver(ref BuilderContext context)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = context.Type.GetTypeInfo();
            if (typeInfo.IsInterface)
#else
            if (context.Type.IsInterface)
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructInterface, c.Type));

                    return c.Existing;
                };
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsAbstract)
#else
            if (context.Type.IsAbstract)
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructAbstractClass, c.Type));

                    return c.Existing;
                };
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsSubclassOf(typeof(Delegate)))
#else
            if (context.Type.IsSubclassOf(typeof(Delegate)))
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructDelegate, c.Type));

                    return c.Existing;
                };
            }

            if (context.Type == typeof(string))
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.TypeIsNotConstructable, c.Type));

                    return c.Existing;
                };
            }

            return null;
        }

        #endregion
    }
}
