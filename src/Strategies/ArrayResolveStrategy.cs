using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Strategies
{
    /// <summary>
    /// This strategy is responsible for building Array
    /// </summary>
    public class ArrayResolveStrategy : BuilderStrategy
    {
        #region Fields

        private readonly MethodInfo _resolveMethod;
        private readonly MethodInfo _resolveGenericMethod;

        #endregion


        #region Constructors

        public ArrayResolveStrategy(MethodInfo method, MethodInfo generic)
        {
            _resolveMethod = method;
            _resolveGenericMethod = generic;
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
            if (namedType is ContainerRegistration containerRegistration)
            {
                if (containerRegistration.RegisteredType != containerRegistration.MappedToType ||
                    null != injectionMembers && injectionMembers.Any(i => i is InjectionFactory))
                    return false;
            }

            return namedType is InternalRegistration registration && null != registration.Type &&
                   registration.Type.IsArray && registration.Type.GetArrayRank() == 1;
        }

        #endregion


        #region Build

        public override void PreBuildUp<TContext>(ref TContext context)
        {
            var plan = context.Registration.Get<ResolveDelegate<TContext>>();
            if (plan == null)
            {
                var typeArgument = context.OriginalBuildKey.Type.GetElementType();
                var type = ((UnityContainer)context.Container).GetFinalType(typeArgument);

                if (type != typeArgument)
                {
                    var method = (ResolveArrayDelegate<TContext>)_resolveGenericMethod
                        .MakeGenericMethod(typeof(TContext), typeArgument)
                        .CreateDelegate(typeof(ResolveArrayDelegate<TContext>));
                    plan = (ref TContext c) => method(ref c, type);
                }
                else
                {
                    plan = (ResolveDelegate<TContext>)_resolveMethod
                        .MakeGenericMethod(typeof(TContext), typeArgument)
                        .CreateDelegate(typeof(ResolveDelegate<TContext>));
                }

                context.Registration.Set(typeof(ResolveDelegate<TContext>), plan);
            }

            context.Existing = plan(ref context);
            context.BuildComplete = true;
        }

        #endregion


        #region Nested Types

        private delegate object ResolveArrayDelegate<TContext>(ref TContext context, Type type)
            where TContext : IBuilderContext;

        #endregion
    }
}
