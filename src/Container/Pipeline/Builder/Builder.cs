using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
        where TContext : IBuilderContext
    {
        #region Fields

        private int _index;
        private object?[]? _analytics;

        private readonly IntPtr _context;
        private readonly BuilderStrategy[] _strategies;

        #endregion


        #region Constructors

        public PipelineBuilder(ref TContext context)
        {
            unsafe
            {
                _context = new IntPtr(Unsafe.AsPointer(ref context));
            }

            _index = 0;
            _analytics = null;
            _strategies = ((Policies<TContext>)context.Policies).StrategiesChain.Values.ToArray();
        }

        public PipelineBuilder(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
        {
            _index = 0;
            _analytics = null;
            _strategies = chain.Values.ToArray();
        }

        #endregion
    }
}
