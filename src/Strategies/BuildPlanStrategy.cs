using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
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
        public override void PreBuildUp(ref BuilderContext context)
        {
            var resolver = context.Registration.Get<ResolveDelegate<BuilderContext>>();
            if (null == resolver)
            {
                // Legacy support
                var plan = context.Registration.Get<IBuildPlanPolicy>() ?? 
                           (IBuildPlanPolicy)(
                               context.Get<IBuildPlanPolicy>(context.Type, string.Empty) ??
                               GetGeneric(ref context, typeof(IBuildPlanPolicy),
                                   context.OriginalBuildKey.Type, context.OriginalBuildKey.Name));

                if (plan == null || plan is OverriddenBuildPlanMarkerPolicy)
                {
                    var planCreator = context.Registration.Get<IBuildPlanCreatorPolicy>() ?? 
                                      CheckIfOpenGeneric(context.Registration) ??
                                      GetPolicy<IBuildPlanCreatorPolicy>(ref context, context.Type, context.Name);

                    if (planCreator != null)
                    {
                        plan = planCreator.CreatePlan(ref context, context.Type, context.Name);

                        if (plan is IResolve policy)
                            context.Registration.Set(typeof(ResolveDelegate<BuilderContext>), (ResolveDelegate<BuilderContext>)policy.Resolve);
                        else
                            context.Registration.Set(typeof(IBuildPlanPolicy), plan);
                    }
                    else
                        throw new ResolutionFailedException(context.Type, context.Name, "Failed to find Build Plan Creator Policy");
                }

                plan?.BuildUp(ref context);
            }
            else
            {
                context.Existing = resolver(ref context);
            }
        }

        #endregion


        #region Implementation

        public static TPolicyInterface GetPolicy<TPolicyInterface>(ref BuilderContext context, Type type, string name)
        {
            return (TPolicyInterface)(GetGeneric(ref context, typeof(TPolicyInterface), type, name) ??
                                      context.Get(null, null, typeof(TPolicyInterface)));    // Nothing! Get Default
        }

        private static object GetGeneric(ref BuilderContext context, Type policyInterface, Type type, string name)
        {
            // Check if generic
            if (type.GetTypeInfo().IsGenericType)
            {
                var newType = type.GetGenericTypeDefinition();
                return context.Get(newType, name, policyInterface) ??
                       context.Get(newType, string.Empty, policyInterface);
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
