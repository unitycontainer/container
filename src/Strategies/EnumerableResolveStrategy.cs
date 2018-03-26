using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Strategies
{
    /// <summary>
    /// This strategy is responsible for building IEnumerable<>
    /// </summary>
    public class EnumerableResolveStrategy : BuilderStrategy
    {
        #region Fields

        private readonly MethodInfo _resolveMethod;
        private readonly MethodInfo _resolveLazyMethod;

        #endregion


        #region Constructors

        public EnumerableResolveStrategy(MethodInfo method, MethodInfo lazy)
        {
            _resolveMethod = method;
            _resolveLazyMethod = lazy;
        }

        #endregion


        #region Build

        public override void PreBuildUp(IBuilderContext context)
        {
            var plan = context.Registration.Get<IBuildPlanPolicy>();
            if (plan == null)
            {
                var args = context.BuildKey.Type.GetTypeInfo().GenericTypeArguments.First();
                var info = args.GetTypeInfo();
                var buildMethod = info.IsGenericType && typeof(Lazy<>) == info.GetGenericTypeDefinition()
                                ? _resolveLazyMethod.MakeGenericMethod(args).CreateDelegate(typeof(DynamicBuildPlanMethod))
                                : _resolveMethod.MakeGenericMethod(args).CreateDelegate(typeof(DynamicBuildPlanMethod));

                plan = new DynamicMethodBuildPlan((DynamicBuildPlanMethod)buildMethod);
                context.Registration.Set(typeof(IBuildPlanPolicy), plan);
            }

            plan?.BuildUp(context);
            context.BuildComplete = true;
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
             return namedType is InternalRegistration registration &&
                    registration.Type.GetTypeInfo().IsGenericType && 
                    typeof(IEnumerable<>) == registration.Type.GetGenericTypeDefinition() 
                ? true : false;
        }

        #endregion
    }
}
