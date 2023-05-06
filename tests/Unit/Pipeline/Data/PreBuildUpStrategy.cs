using System.Collections.Generic;
using Unity.Builder;

namespace Pipeline
{
    public class PreBuildUpStrategy 
    {
        public static readonly string PreName = $"{nameof(PreBuildUpStrategy)}.PreBuildUp";
        public static readonly string PostName = $"{nameof(PreBuildUpStrategy)}.PreBuildUp";

        public void PreBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            ((IList<string>)context.Existing).Add(PreName);
        }

        public object Analyze<TContext>(ref TContext context)
            => nameof(PreBuildUpStrategy);
    }
}
