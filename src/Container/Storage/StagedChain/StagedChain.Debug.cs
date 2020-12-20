using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Unity.Storage
{
    [DebuggerDisplay("StagedChain: Type = {Type.Name}, Count = {Count}")]
    [DebuggerTypeProxy(typeof(StagedChain<,>.StagedChainProxy))]
    public partial class StagedChain<TStageEnum, TStrategyType>
    {
        public class StagedChainProxy
        {
            public StagedChainProxy(StagedChain<TStageEnum, TStrategyType> chain)
            {
                var names = Enum.GetNames(typeof(TStageEnum));
                
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
                public TStrategyType Strategy;
            }
        }
    }
}
