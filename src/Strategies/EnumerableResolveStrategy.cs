using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Strategies
{
    /// <summary>
    /// This strategy is responsible for building IEnumerable
    /// </summary>
    public class EnumerableResolveStrategy : BuilderStrategy
    {
        #region Fields

        private readonly MethodInfo _resolveMethod;
        private readonly MethodInfo _resolveGenericMethod;

        #endregion


        #region Constructors

        public EnumerableResolveStrategy(MethodInfo method, MethodInfo generic)
        {
            _resolveMethod = method;
            _resolveGenericMethod = generic;
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, Type type, InternalRegistration registration, params InjectionMember[] injectionMembers)
        {
            if (registration is ContainerRegistration containerRegistration)
            {
                if (type != containerRegistration.Type ||
#pragma warning disable CS0618 // TODO: InjectionFactory
                    null != injectionMembers && injectionMembers.Any(i => i is InjectionFactory))
#pragma warning restore CS0618 
                    return false;
            }
#if NETSTANDARD1_0 || NETCOREAPP1_0
            return type.GetTypeInfo().IsGenericType && typeof(IEnumerable<>) == type.GetGenericTypeDefinition();
#else
            return type.IsGenericType && typeof(IEnumerable<>) == type.GetGenericTypeDefinition();
#endif
        }

        #endregion


        #region Build

        public override void PreBuildUp(ref BuilderContext context)
        {
            var plan = context.Registration.Get<ResolveDelegate<BuilderContext>>();
            if (plan == null)
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
                var typeArgument = context.Type.GetTypeInfo().GenericTypeArguments.First();
#else
                var typeArgument = context.Type.GenericTypeArguments.First();
#endif
                var type = ((UnityContainer)context.Container).GetFinalType(typeArgument);
                if (type != typeArgument)
                {
                    var method = (ResolveEnumerableDelegate)_resolveGenericMethod
                        .MakeGenericMethod(typeArgument)
                        .CreateDelegate(typeof(ResolveEnumerableDelegate));
                    plan = (ref BuilderContext c) => method(ref c, type);
                }
                else
                {
                    plan = (ResolveDelegate<BuilderContext>)
                        _resolveMethod.MakeGenericMethod(typeArgument)
                                      .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
                }

                context.Registration.Set(typeof(ResolveDelegate<BuilderContext>), plan);
            }

            context.Existing = plan(ref context);
            context.BuildComplete = true;
        }

        #endregion


        #region Nested Types

        private delegate object ResolveEnumerableDelegate(ref BuilderContext context, Type type);

        #endregion
    }
}
