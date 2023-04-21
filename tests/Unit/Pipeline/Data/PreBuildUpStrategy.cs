using System.Collections.Generic;
using Unity.Strategies;

namespace Pipeline
{
    public class PreBuildUpStrategy : BuilderStrategy
    {
        public static readonly string PreName  = $"{nameof(PreBuildUpStrategy)}.{nameof(PreBuildUp)}";
        public static readonly string PostName = $"{nameof(PreBuildUpStrategy)}.{nameof(PostBuildUp)}";

        public override void PreBuildUp<TContext>(ref TContext context)
        {
            ((IList<string>)context.Existing).Add(PreName);
        }

        public object Analyze<TContext>(ref TContext context)
            => nameof(PreBuildUpStrategy);
    }
}
