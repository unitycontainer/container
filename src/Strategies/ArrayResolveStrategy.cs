using System;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Strategies
{
    /// <summary>
    /// This strategy is responsible for building Array
    /// </summary>
    public class ArrayResolveStrategy : BuilderStrategy
    {
        #region Fields

        private readonly MethodInfo _resolveMethod;
        private readonly MethodInfo _resolveLazyMethod;

        #endregion


        #region Constructors

        public ArrayResolveStrategy(MethodInfo method, MethodInfo lazy)
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
                var type = context.OriginalBuildKey.Type.GetElementType();
                var info = type.GetTypeInfo();
                var buildMethod = info.IsGenericType && typeof(Lazy<>) == info.GetGenericTypeDefinition()
                                ? _resolveLazyMethod.MakeGenericMethod(type).CreateDelegate(typeof(DynamicBuildPlanMethod))
                                : _resolveMethod.MakeGenericMethod(type).CreateDelegate(typeof(DynamicBuildPlanMethod));

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
            return  namedType is InternalRegistration registration && 
                    registration.Type.IsArray && registration.Type.GetArrayRank() == 1
                ? true : false;
        }

        #endregion
    }
}
