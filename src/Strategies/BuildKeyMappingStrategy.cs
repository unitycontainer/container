using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Policy.Mapping;
using Unity.Registration;

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
                context.Registration.Set(typeof(IBuildPlanPolicy), 
                    new DynamicMethodBuildPlan(c => 
                    {
                        ((BuilderContext)c).ChildContext = new BuilderContext(c, context.BuildKey.Type, context.BuildKey.Name);
                        ((BuilderContext)c.ChildContext).BuildUp();

                        c.Existing = c.ChildContext.Existing;
                        c.BuildComplete = null != context.Existing;

                        var plan = c.ChildContext.Registration.Get(typeof(IBuildPlanPolicy));
                        if (null != plan) context.Registration.Set(typeof(IBuildPlanPolicy), plan);

                        ((BuilderContext)c).ChildContext = null;
                    }));
            }
        }


        public override void PostBuildUp(IBuilderContext context)
        {
            if (context.Registration is InternalRegistration registration && 
                null != registration.BuildChain &&
                null != context.Registration.Get<IBuildPlanPolicy>())
            {
                var chain = new List<BuilderStrategy>();
                var strategies = registration.BuildChain;

                for (var i = 0; i < strategies.Count; i++)
                {
                    var strategy = strategies[i];
                    if (!(strategy is BuildKeyMappingStrategy))
                        chain.Add(strategy);
                }

                registration.BuildChain = chain;
            }
        }
        
        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
            switch (namedType)
            {
                case ContainerRegistration registration:
                    return AnalysStaticRegistration(container, registration, injectionMembers);

                case InternalRegistration registration:
                    return AnalysDynamicRegistration(container, registration);

                default:
                    return false;
            }
        }

        private bool AnalysStaticRegistration(IUnityContainer container, ContainerRegistration registration, params InjectionMember[] injectionMembers)
        {
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

        private bool AnalysDynamicRegistration(IUnityContainer container, InternalRegistration registration)
        {
            return registration.Type.GetTypeInfo().IsGenericType;
        }


        #endregion
    }
}
