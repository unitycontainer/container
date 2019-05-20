using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
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
            // Get resolver if already created
            var resolver = context.Registration.Get<ResolveDelegate<BuilderContext>>() ?? (ResolveDelegate<BuilderContext>) 
                                   GetGeneric(ref context, typeof(ResolveDelegate<BuilderContext>));

            if (null == resolver)
            {
                // Check if can create at all

#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (!(context.Registration is ContainerRegistration) &&  context.RegistrationType.GetTypeInfo().IsGenericTypeDefinition)
#else
                if (!(context.Registration is ContainerRegistration) && context.RegistrationType.IsGenericTypeDefinition)
#endif
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "The type {0} is an open generic type. An open generic type cannot be resolved.",
                        context.RegistrationType.FullName), new InvalidRegistrationException());
                }
                else if (context.Type.IsArray && context.Type.GetArrayRank() > 1)
                {
                    var message = $"Invalid array {context.Type}. Only arrays of rank 1 are supported";
                    throw new ArgumentException(message, new InvalidRegistrationException());
                }

                // Get resolver factory
                var factory = context.Registration.Get<ResolveDelegateFactory>() ?? (ResolveDelegateFactory)(
                              context.Get(context.Type, UnityContainer.All, typeof(ResolveDelegateFactory)) ??
                              GetGeneric(ref context, typeof(ResolveDelegateFactory)) ?? 
                              context.Get(null, null, typeof(ResolveDelegateFactory)));

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


        #region Implementation

        protected static TPolicyInterface Get_Policy<TPolicyInterface>(ref BuilderContext context, Type type, string name)
        {
            return (TPolicyInterface)(GetGeneric(ref context, typeof(TPolicyInterface), type, name) ??
                                      context.Get(null, null, typeof(TPolicyInterface)));    // Nothing! Get Default
        }

        protected static object GetGeneric(ref BuilderContext context, Type policyInterface)
        {
            if (context.Registration is ContainerRegistration registration && null != context.Type)
            {
                // Check if generic
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (context.Type.GetTypeInfo().IsGenericType)
#else
                if (context.Type.IsGenericType)
#endif
                {
                    var newType = context.Type.GetGenericTypeDefinition();
                    return context.Get(newType, context.Name, policyInterface) ??
                           context.Get(newType, UnityContainer.All, policyInterface);
                }
            }
            else
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
                           context.Get(newType, UnityContainer.All, policyInterface);
                }
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
                       context.Get(newType, UnityContainer.All, policyInterface);
            }

            return null;
        }

        #endregion
    }
}
