using System;
using System.Diagnostics;
using Unity.Extension;

namespace Unity.Storage
{
    [DebuggerDisplay("StagedChain: Type = {Type.Name}, Count = {Count}")]
    [DebuggerTypeProxy(typeof(StagedChainProxy))]
    public partial class StagedStrategyChain
    {
        public class StagedChainProxy
        {
            public StagedChainProxy(StagedStrategyChain chain)
            {
                var names = Enum.GetNames(typeof(UnityBuildStage));
                
                Entries = new Entry[names.Length];

                for (int i = 0; i < names.Length; i++)
                {
                    ref var entry = ref Entries[i];
                    entry.Stage   = names[i];
                    entry.Strategy = chain._stages[i]!;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Entry[] Entries { get; }

            [DebuggerDisplay("{Strategy}", Name = "{Stage,nq}")]
            public struct Entry
            {
                public string Stage;
                public BuilderStrategy Strategy;
            }
        }
    }
}
