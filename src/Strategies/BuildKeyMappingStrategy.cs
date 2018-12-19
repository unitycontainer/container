using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Policy.Mapping;
using Unity.Registration;
using Unity.Storage;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy
    {
        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
            switch (namedType)
            {
                case ContainerRegistration registration:
                    return AnalyzeStaticRegistration(registration, injectionMembers);

                case InternalRegistration registration:
                    return AnalyseDynamicRegistration(registration);

                default:
                    return false;
            }
        }

        private bool AnalyzeStaticRegistration(ContainerRegistration registration, params InjectionMember[] injectionMembers)
        {
            // Validate input  
            if (null == registration.MappedToType || registration.RegisteredType == registration.MappedToType) return false;

            // Require Re-Resolve if no injectors specified
            var buildRequired = registration.LifetimeManager is PerResolveLifetimeManager ||
                                (injectionMembers?.Any(m => m.BuildRequired) ?? false);

            // Set mapping policy
            var policy = registration.RegisteredType.GetTypeInfo().IsGenericTypeDefinition &&
                         registration.MappedToType.GetTypeInfo().IsGenericTypeDefinition
                ? new GenericTypeBuildKeyMappingPolicy(registration.MappedToType, buildRequired)
                : (IBuildKeyMappingPolicy)new BuildKeyMappingPolicy(registration.MappedToType, buildRequired);
            registration.Set(typeof(IBuildKeyMappingPolicy), policy);

            return true;
        }

        private static bool AnalyseDynamicRegistration(InternalRegistration registration)
        {
            return null != registration.Type && registration.Type.GetTypeInfo().IsGenericType;
        }


        #endregion


        #region Build

        /// <summary>
        /// Called during the chain of responsibility for a build operation.  Looks for the <see cref="IBuildKeyMappingPolicy"/>
        /// and if found maps the build key for the current operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            if (context.Registration is ContainerRegistration registration && 
                registration.RegisteredType == registration.MappedToType)
                return;
                
            IBuildKeyMappingPolicy policy = ((IPolicySet)context.Registration).Get<IBuildKeyMappingPolicy>() 
                                          ?? (context.Registration.Type.GetTypeInfo().IsGenericType 
                                          ? context.Get<IBuildKeyMappingPolicy>(context.Registration.Type.GetGenericTypeDefinition(), 
                                                                                context.Registration.Name) 
                                          : null);
            if (null == policy) return;

            context.Type = policy.Map(ref context);

            if (!policy.RequireBuild && ((UnityContainer)context.Container).RegistrationExists(context.Type, context.Name))
            {
                var type = context.Type;
                var name = context.Name;

                ((IPolicySet)context.Registration).Set(typeof(IBuildPlanPolicy), 
                    new DynamicMethodBuildPlan((ResolveDelegate<BuilderContext>) ResolveDelegate));

                object ResolveDelegate(ref BuilderContext c) => c.Existing = c.Resolve(type, name);
            }
        }


        public override void PostBuildUp(ref BuilderContext context)
        {
            if (((IPolicySet)context.Registration) is InternalRegistration registration && 
                null != registration.BuildChain &&
                null != ((IPolicySet)context.Registration).Get<IBuildPlanPolicy>())
            {
                var chain = new List<BuilderStrategy>();
                var strategies = registration.BuildChain;

                for (var i = 0; i < strategies.Length; i++)
                {
                    var strategy = strategies[i];
                    if (!(strategy is BuildKeyMappingStrategy))
                        chain.Add(strategy);
                }

                registration.BuildChain = chain.ToArray();
            }
        }
        
        #endregion
    }
}
