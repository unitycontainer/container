using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.Policies
{
    public class ResolveMappingPolicy : BuildKeyMappingPolicy, IDependencyResolverPolicy
    {
        public ResolveMappingPolicy(NamedTypeBuildKey newBuildKey)
            : base(newBuildKey)
        {
                
        }

        public object Resolve(IBuilderContext context)
        {
            return context.NewBuildUp(Map(context.BuildKey, context));

        }
    }
}
