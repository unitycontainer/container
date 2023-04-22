using System.Collections.Generic;
using Unity.Builder;

namespace Pipeline
{
    public class PostBuildUpStrategy 
    {
        public static readonly string PreName  = $"{nameof(PostBuildUpStrategy)}.PreBuildUp";
        public static readonly string PostName = $"{nameof(PostBuildUpStrategy)}.PostBuildUp";

        public void PostBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            ((IList<string>)context.Existing).Add(PostName);
        }

        public object Analyze<TContext>(ref TContext context)
            => nameof(PostBuildUpStrategy);
    }
}
