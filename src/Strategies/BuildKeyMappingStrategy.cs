using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;
using Unity.Policy.Mapping;
using Unity.Registration;
using Unity.Strategy;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy, IRegisterTypeStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation.  Looks for the <see cref="IBuildKeyMappingPolicy"/>
        /// and if found maps the build key for the current operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override object PreBuildUp(IBuilderContext context)
        {
            IBuildKeyMappingPolicy policy = context.PersistentPolicies.Get<IBuildKeyMappingPolicy>(context.OriginalBuildKey.Type, 
                                                                                                   context.OriginalBuildKey.Name, out _) 
                                          ?? (context.OriginalBuildKey.Type.GetTypeInfo().IsGenericType 
                                          ? context.Policies.Get<IBuildKeyMappingPolicy>(context.OriginalBuildKey.Type.GetGenericTypeDefinition(), 
                                                                                         context.OriginalBuildKey.Name, out _) 
                                          : null);

            if (null == policy) return null;

            context.BuildKey = policy.Map(context.BuildKey, context);
            return null;
        }

        #endregion


        #region IRegisterTypeStrategy

        public void RegisterType(IContainerContext context, Type typeFrom, Type typeTo, string name, 
                                 LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            // Validate imput
            if (typeFrom == null || typeFrom == typeTo) return;

            // Set mapping policy
            var policy = typeFrom.GetTypeInfo().IsGenericTypeDefinition && typeTo.GetTypeInfo().IsGenericTypeDefinition
                       ? new GenericTypeBuildKeyMappingPolicy(typeTo, name)
                       : (IBuildKeyMappingPolicy)new BuildKeyMappingPolicy(typeTo, name);
            context.Policies.Set(typeFrom, name, typeof(IBuildKeyMappingPolicy), policy);

            // Require Re-Resolve if no injectors specified
            var members = null == injectionMembers ? new InjectionMember[0] : injectionMembers;
            var overrides = members.Where(m => m is InjectionConstructor || m is InjectionMethod || m is InjectionProperty).Any();
            if (lifetimeManager is IRequireBuildUpPolicy || overrides) return;
            context.Policies.Set(typeFrom, name, typeof(IBuildPlanPolicy), new ResolveBuildUpPolicy());
        }

        #endregion

    }
}
