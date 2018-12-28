using System;
using System.Linq;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
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
        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, Type type, InternalRegistration registration, params InjectionMember[] injectionMembers)
        {
            // Require Re-Resolve if no injectors specified
            registration.BuildRequired = (injectionMembers?.Any(m => m.BuildRequired) ?? false) ||
                                          registration is ContainerRegistration cr && cr.LifetimeManager is PerResolveLifetimeManager;

            return true;
        }

        #endregion


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
                // TODO: Verify and optimize getting default
                var plan = context.Registration.Get<IBuildPlanPolicy>() ??
                           (IBuildPlanPolicy)(
                               context.Get(context.Type, string.Empty, typeof(IBuildPlanPolicy)) ??
                               GetGeneric(ref context, typeof(IBuildPlanPolicy)));

                if (plan == null)
                {
                    // Check if can create 
#if NETCOREAPP1_0 || NETSTANDARD1_0
                    if (!(context.Registration is ContainerRegistration) &&  context.RegistrationType.GetTypeInfo().IsGenericTypeDefinition)
#else
                    if (!(context.Registration is ContainerRegistration) && context.RegistrationType.IsGenericTypeDefinition)
#endif
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            Constants.CannotResolveOpenGenericType, context.RegistrationType.FullName));
                    }

                    // Create plan 
                    var factory = context.Registration.Get<ResolveDelegateFactory>() ??
                                      Get_Policy<ResolveDelegateFactory>(ref context, context.Type, context.Name);

                    if (null != factory)
                    {
                        resolver = factory(ref context);

                        context.Registration.Set(typeof(ResolveDelegate<BuilderContext>), resolver);
                        context.Existing = resolver(ref context);

                        return;
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

        protected static TPolicyInterface Get_Policy<TPolicyInterface>(ref BuilderContext context, Type type, string name)
        {
            return (TPolicyInterface)(GetGeneric(ref context, typeof(TPolicyInterface), type, name) ??
                                      context.Get(null, null, typeof(TPolicyInterface)));    // Nothing! Get Default
        }

        protected static object GetGeneric(ref BuilderContext context, Type policyInterface)
        {
            // Check if generic
#if NETCOREAPP1_0 || NETSTANDARD1_0
            if (context.RegistrationType.GetTypeInfo().IsGenericType)
#else
            if (context.RegistrationType.IsGenericType)
#endif
            {
                var newType = context.RegistrationType.GetGenericTypeDefinition();
                return context.Get(newType, context.Name, policyInterface) ??
                       context.Get(newType, string.Empty, policyInterface);
            }

            return null;
        }

        protected static object GetGeneric(ref BuilderContext context, Type policyInterface, Type type, string name)
        {
            // Check if generic
#if NETCOREAPP1_0 || NETSTANDARD1_0
            if (type.GetTypeInfo().IsGenericType)
#else
            if (type.IsGenericType)
#endif
            {
                var newType = type.GetGenericTypeDefinition();
                return context.Get(newType, name, policyInterface) ??
                       context.Get(newType, string.Empty, policyInterface);
            }

            return null;
        }

        #endregion
    }
}
