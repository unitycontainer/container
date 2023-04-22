using System.Collections.Generic;
using Unity.Builder;

namespace Pipeline
{
    public class BothStrategies 
    {
        public static readonly string PreName  = $"{nameof(BothStrategies)}.{nameof(PreBuildUp)}";
        public static readonly string PostName = $"{nameof(BothStrategies)}.{nameof(PostBuildUp)}";

        public void PreBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            ((IList<string>)context.Existing).Add(PreName);
        }

        public void PostBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            ((IList<string>)context.Existing).Add(PostName);
        }

        public object Analyze<TContext>(ref TContext context)
            => nameof(BothStrategies);
    }
}
