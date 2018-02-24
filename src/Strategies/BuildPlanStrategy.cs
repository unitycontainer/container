using System;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;

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
        public override void PreBuildUp(IBuilderContext context)
        {
            var plan = GetPolicy<IBuildPlanPolicy>(context.Policies, context.OriginalBuildKey, out var buildPlanLocation);

            if (plan == null || plan is OverriddenBuildPlanMarkerPolicy)
            {
                var planCreator = GetPolicy<IBuildPlanCreatorPolicy>(context.Policies, context.BuildKey, out var creatorLocation);
                if (planCreator != null)
                {
                    plan = planCreator.CreatePlan(context, context.BuildKey);
                    (buildPlanLocation ?? creatorLocation).Set(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, typeof(IBuildPlanPolicy), plan);
                }
                else
                    throw new ResolutionFailedException(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, null, context);
            }

            plan?.BuildUp(context);
        }

        #endregion


        #region Implementation


        public static TPolicyInterface GetPolicy<TPolicyInterface>(IPolicyList list, INamedType buildKey, out IPolicyList containingPolicyList)
        {
            return (TPolicyInterface)(list.Get(buildKey.Type, buildKey.Name, typeof(TPolicyInterface), out containingPolicyList)     ??
                                      GetExtended(list, typeof(TPolicyInterface), buildKey, buildKey.Type, out containingPolicyList) ??
                                      list.Get(null, null, typeof(TPolicyInterface), out containingPolicyList));    // Nothing! Get Default
        }

        private static IBuilderPolicy GetExtended(IPolicyList list, Type policyInterface, INamedType buildKey, Type buildType, out IPolicyList containingPolicyList)
        {
            containingPolicyList = null;
            if (null == buildType) return null;

            // Check if generic
            if (buildType.GetTypeInfo().IsGenericType)
            {
                var newType = buildType.GetGenericTypeDefinition();
                return list.Get(newType, buildKey.Name, policyInterface, out containingPolicyList) ??
                       list.Get(newType, string.Empty, policyInterface, out containingPolicyList);
            }

            // Check if array
            if (buildType.IsArray && buildType.GetArrayRank() == 1)
            {
                return list.Get(typeof(Array), buildKey.Name, policyInterface, out containingPolicyList) ??
                       list.Get(typeof(Array), string.Empty, policyInterface, out containingPolicyList);
            }

            // Check default for type
            return list.Get(buildType, string.Empty, policyInterface, out containingPolicyList);
        }

        #endregion
    }
}
