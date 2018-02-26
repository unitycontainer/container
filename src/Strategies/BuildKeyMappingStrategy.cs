using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Policy.Mapping;
using Unity.Registration;
using Unity.Strategy;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy
    {
        #region Build

        /// <summary>
        /// Called during the chain of responsibility for a build operation.  Looks for the <see cref="IBuildKeyMappingPolicy"/>
        /// and if found maps the build key for the current operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if (context.OriginalBuildKey is ContainerRegistration registration && 
                registration.RegisteredType == registration.MappedToType)
                return;
                
            IBuildKeyMappingPolicy policy = context.Registration.Get<IBuildKeyMappingPolicy>() 
                                          ?? (context.OriginalBuildKey.Type.GetTypeInfo().IsGenericType 
                                          ? context.Policies.Get<IBuildKeyMappingPolicy>(context.OriginalBuildKey.Type.GetGenericTypeDefinition(), 
                                                                                         context.OriginalBuildKey.Name, out _) 
                                          : null);
            if (null == policy) return;

            context.BuildKey = policy.Map(context.BuildKey, context);

            if (!policy.RequireBuild && context.Container.IsRegistered(context.BuildKey.Type, context.BuildKey.Name))
            {
                context.Existing = context.NewBuildUp(context.BuildKey.Type, context.BuildKey.Name);
                context.BuildComplete = null != context.Existing;
            }
        }


        public override void PostBuildUp(IBuilderContext context)
        {
            if (context.Registration is InternalRegistration registration && 
                null != context.Registration.Get<IBuildPlanPolicy>())
            {
                registration.BuildChain = registration.BuildChain
                                                      .Where(strategy => !(strategy is BuildKeyMappingStrategy))
                                                      .ToArray();
            }
        }
        
        #endregion


        #region Registration and Analysis

        public override bool RegisterType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
            var registration = (ContainerRegistration)namedType;

            // Validate imput
            if (null == registration.MappedToType || registration.RegisteredType == registration.MappedToType) return false;

            // Require Re-Resolve if no injectors specified
            var buildRequired = registration.LifetimeManager is IRequireBuildUpPolicy ||
                (null == injectionMembers ? false : injectionMembers.Any(m => m.BuildRequired));

            // Set mapping policy
            var policy = registration.RegisteredType.GetTypeInfo().IsGenericTypeDefinition && 
                         registration.MappedToType.GetTypeInfo().IsGenericTypeDefinition
                       ? new GenericTypeBuildKeyMappingPolicy(registration.MappedToType, registration.Name, buildRequired)
                       : (IBuildKeyMappingPolicy)new BuildKeyMappingPolicy(registration.MappedToType, registration.Name, buildRequired);
            registration.Set(typeof(IBuildKeyMappingPolicy), policy);

            return true;
        }

        #endregion




        #region IRegisterTypeStrategy

        public void RegisterType(IContainerContext context, Type typeFrom, Type typeTo, string name, 
                                 LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
        }

        #endregion

    }
}
