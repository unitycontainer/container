using System;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Policies<TProcessor, TStage>
    {
        #region Constants

        private const int HashMask = unchecked((int)(uint.MaxValue >> 1));
        private const float LoadFactor = 0.72f;

        #endregion


        #region Fields

        private int _max;
        private int _count;
        private int _prime = 1;
        private Entry[]    _policies;
        private Metadata[] _metadata;

        #endregion


        #region Constructors

        public Policies()
        {
            // Build Chains
            InstancePipeline = new StagedStrategyChain<TProcessor, TStage>();
            FactoryPipeline  = new StagedStrategyChain<TProcessor, TStage>();
            TypePipeline     = new StagedStrategyChain<TProcessor, TStage>();

            // Initialize arrays
            var size = Prime.Numbers[_prime];
            _max = Math.Min(100, (int)(size * LoadFactor));
            _policies = new Entry[size];
            _metadata = new Metadata[size];
        }

        #endregion


        #region Implementation

        private void Expand()
        {
            var size = Prime.Numbers[++_prime];
            _max = Math.Min(50, (int)(size * LoadFactor));

            Array.Resize(ref _policies, size);
            var metadata = new Metadata[size];

            for (var index = 1; index <= _count; index++)
            {
                var offset = _policies[index].HashCode % size;
                ref var bucket = ref _metadata[offset].Bucket;

                metadata[index].Next = bucket;
                bucket = index;
            }
            
            _metadata = metadata;
        }

        private struct Entry
        {
            public int     HashCode;
            public Type?   Type;
            public Type    Policy;
            public object? Value;
        }

        #endregion
    }
}
