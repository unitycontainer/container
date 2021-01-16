using System;
using System.Runtime.CompilerServices;
using Unity.Extension;

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
            _strategies = ((Policies<TContext>)context.Policies).TypeChain.ToArray();
        }

        #endregion
    }
}
