using System.Collections.Generic;
using Unity.Extension;

namespace Pipeline
{
    public class BothStrategies : BuilderStrategy
    {
        public static readonly string PreName  = $"{nameof(BothStrategies)}.{nameof(PreBuildUp)}";
        public static readonly string PostName = $"{nameof(BothStrategies)}.{nameof(PostBuildUp)}";

        public override void PreBuildUp<TContext>(ref TContext context)
        {
            ((IList<string>)context.Existing).Add(PreName);
        }

        public override void PostBuildUp<TContext>(ref TContext context)
        {
            ((IList<string>)context.Existing).Add(PostName);
        }

        public override object Analyse<TContext>(ref TContext context)
            => nameof(BothStrategies);
    }
}
