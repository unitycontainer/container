using System.Collections.Generic;
using Unity.Extension;

namespace Pipeline
{
    public class PostBuildUpStrategy : BuilderStrategy
    {
        public static readonly string PreName  = $"{nameof(PostBuildUpStrategy)}.{nameof(PreBuildUp)}";
        public static readonly string PostName = $"{nameof(PostBuildUpStrategy)}.{nameof(PostBuildUp)}";

        public override void PostBuildUp<TContext>(ref TContext context)
        {
            ((IList<string>)context.Existing).Add(PostName);
        }

        public override object Analyse<TContext>(ref TContext context)
            => nameof(PostBuildUpStrategy);
    }
}
