using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

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

        public override bool RequiredToBuildType(IUnityContainer container, Type type, ImplicitRegistration registration, params InjectionMember[]? injectionMembers)
        {
            // Require Re-Resolve if no injectors specified
            registration.BuildRequired = (injectionMembers?.Any(m => m.BuildRequired) ?? false) ||
                                          registration is ExplicitRegistration cr && cr.LifetimeManager is PerResolveLifetimeManager;

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
            var resolver = context.GetResolver();

            if (null == resolver)
            {
                // Check if can be created
                if (!(context.Registration is ExplicitRegistration) &&  
#if NETCOREAPP1_0 || NETSTANDARD1_0
                      context.RegistrationType.GetTypeInfo().IsGenericTypeDefinition)
#else
                      context.RegistrationType.IsGenericTypeDefinition)
#endif
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "The type {0} is an open generic type. An open generic type cannot be resolved.",
                        context.RegistrationType.FullName));
                }

                // Get resolver factory
                var factory = context.GetFactory();

                // Create plan 
                if (null != factory)
                {
                    resolver = factory(ref context);

                    context.Registration.Set(typeof(ResolveDelegate<BuilderContext>), resolver);
                    context.Existing = resolver(ref context);
                }
                else
                    throw new ResolutionFailedException(context.Type, context.Name, $"Failed to find Resolve Delegate Factory for Type {context.Type}");
            }
            else
            {
                // Plan has been already created, just build the object
                context.Existing = resolver(ref context);
            }
        }

        #endregion
    }
}
