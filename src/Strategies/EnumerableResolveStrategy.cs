using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Injection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Registration;

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
        delegate void ResolveGenericEnumerable(IBuilderContext context, Type type);

        #endregion


        #region Constructors

        public EnumerableResolveStrategy(MethodInfo method, MethodInfo generic)
        {
            _resolveMethod = method;
            _resolveGenericMethod = generic;
        }

        #endregion


        #region Build

        public override void PreBuildUp(IBuilderContext context)
        {
            var plan = context.Registration.Get<IBuildPlanPolicy>();
            if (plan == null)
            {
                var typeArgument = context.BuildKey.Type.GetTypeInfo().GenericTypeArguments.First();
                var type = ((UnityContainer) context.Container).GetFinalType(typeArgument);
                if (type != typeArgument)
                {
                    var method = _resolveGenericMethod.MakeGenericMethod(typeArgument)
                                                      .CreateDelegate(typeof(ResolveGenericEnumerable));

                    plan = new DynamicMethodBuildPlan(c => method.DynamicInvoke(c, type));
                }
                else
                {
                    plan = new DynamicMethodBuildPlan((DynamicBuildPlanMethod) 
                        _resolveMethod.MakeGenericMethod(typeArgument)
                                      .CreateDelegate(typeof(DynamicBuildPlanMethod)));
                }

                context.Registration.Set(typeof(IBuildPlanPolicy), plan);
            }

            plan.BuildUp(context);
            context.BuildComplete = true;
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
            if (namedType is ContainerRegistration containerRegistration)
            {
                if (containerRegistration.RegisteredType != containerRegistration.MappedToType ||
                    null != injectionMembers && injectionMembers.Any(i => i is IInjectionFactory))
                    return false;
            }

             return namedType is InternalRegistration registration &&
                    registration.Type.GetTypeInfo().IsGenericType && 
                    typeof(IEnumerable<>) == registration.Type.GetGenericTypeDefinition();
        }

        #endregion
    }
}
