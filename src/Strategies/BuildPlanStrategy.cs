using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.ObjectBuilder.BuildPlan.ConstructorInvoke;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that will look for a build plan
    /// in the current context. If it exists, it invokes it, otherwise
    /// it creates one and stores it for later, and invokes it.
    /// </summary>
    public class BuildPlanStrategy : BuilderStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            var plan = context.Registration.Get<IBuildPlanPolicy>() ?? (IBuildPlanPolicy)(
                       context.Policies.Get(context.BuildKey.Type, string.Empty, typeof(IBuildPlanPolicy)) ?? 
                       GetGeneric(context.Policies, typeof(IBuildPlanPolicy), 
                                                    context.OriginalBuildKey, 
                                                    context.OriginalBuildKey.Type));

            if (plan == null || plan is OverriddenBuildPlanMarkerPolicy)
            {
                var planCreator = context.Registration.Get<IBuildPlanCreatorPolicy>() ?? CheckIfOpenGeneric(context.Registration) ??
                    GetPolicy<IBuildPlanCreatorPolicy>(context.Policies, context.BuildKey);

                if (planCreator != null)
                {
                    plan = planCreator.CreatePlan(ref context, context.BuildKey);
                    context.Registration.Set(typeof(IBuildPlanPolicy), plan);
                }
                else
                    throw new ResolutionFailedException(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, null, context);
            }

            plan?.BuildUp(ref context);
        }

        #endregion


        #region Implementation

        public static TPolicyInterface GetPolicy<TPolicyInterface>(IPolicyList list, INamedType buildKey)
        {
            return (TPolicyInterface)(GetGeneric(list, typeof(TPolicyInterface), buildKey, buildKey.Type) ??
                                      list.Get(null, null, typeof(TPolicyInterface)));    // Nothing! Get Default
        }

        private static object GetGeneric(IPolicyList list, Type policyInterface, INamedType buildKey, Type buildType)
        {
            // Check if generic
            if (buildType.GetTypeInfo().IsGenericType)
            {
                var newType = buildType.GetGenericTypeDefinition();
                return list.Get(newType, buildKey.Name, policyInterface) ??
                       list.Get(newType, string.Empty, policyInterface);
            }

            return null;
        }

        private static IBuildPlanCreatorPolicy CheckIfOpenGeneric(IPolicySet namedType)
        {
            if (namedType is InternalRegistration registration && !(namedType is ContainerRegistration) && 
                null != registration.Type && registration.Type.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                    Constants.CannotResolveOpenGenericType, registration.Type.FullName));
            }

            return null;
        }

        #endregion
    }
}
