using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Policy;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy
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
    }
}
