using System.Collections.Generic;
using Unity.Extension;

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

        public override object Analyse<TContext>(ref TContext context)
            => nameof(PreBuildUpStrategy);
    }
}
