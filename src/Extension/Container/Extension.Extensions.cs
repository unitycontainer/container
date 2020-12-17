using System.Collections.Generic;
using Unity.Container;

namespace Unity.Extension
{
    public static class ExtensionContextExtensions
    {
        public static void Add(this IDictionary<UnityBuildStage, BuilderStrategy> chain, BuilderStrategy strategy, UnityBuildStage stage)
        {
            chain.Add(stage, strategy);
        }
    }
}
