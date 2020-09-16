using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public partial struct PipelineContext
    {
        #region Fields

        private readonly IntPtr _context;

        public bool IsFaulted;

        #endregion


        #region Constructors

        public PipelineContext(ref ResolutionContext context)
        {
            IsFaulted = false;

            unsafe
            {
                _context = new IntPtr(Unsafe.AsPointer(ref context));
            }

        }

        #endregion

    }
}
