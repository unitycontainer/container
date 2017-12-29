using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;
using Unity.Registration;
using Unity.Strategy;

namespace Unity.ObjectBuilder.Strategies
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy, IRegisterTypeStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation.  Looks for the <see cref="IBuildKeyMappingPolicy"/>
        /// and if found maps the build key for the current operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override object PreBuildUp(IBuilderContext context)
        {
            var policy = context.Policies.Get<IBuildKeyMappingPolicy>(context.BuildKey, out _);
            if (null == policy) return null;

            context.BuildKey = policy.Map(context.BuildKey, context);

            return null;
        }

        public void RegisterType(IContainerContext context, Type typeFrom, Type typeTo, string name, 
                                 LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            var buildType = typeFrom ?? typeTo;

            if (null == typeFrom || typeFrom == typeTo)
            {
                context.Policies.Clear(buildType, name, typeof(IBuildKeyMappingPolicy));
                return;
            }

            var buildKey = new NamedTypeBuildKey(buildType, name);

            if (typeFrom.GetTypeInfo().IsGenericTypeDefinition && typeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                context.Policies.Set<IBuildKeyMappingPolicy>(new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeTo, name)), 
                                                                                                  new NamedTypeBuildKey(typeFrom, name));
            }
            else
            {
                context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(new NamedTypeBuildKey(typeTo, name)), buildKey);
            }

            var members = null == injectionMembers ? new InjectionMember[0] : injectionMembers;
            if (!members.Where(m => m is InjectionConstructor || m is InjectionMethod || m is InjectionProperty).Any() && !(lifetimeManager is IRequireBuildUpPolicy))
                context.Policies.Set<IBuildPlanPolicy>(new ResolveBuildUpPolicy(), buildKey);
        }
    }
}
