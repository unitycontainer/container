using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct PipelineContext
    {

        public ref struct Scope
        {
            #region Fields

            private readonly IntPtr _context;
            private readonly UnityContainer _container;

            #endregion

            #region Constructors

            public Scope(UnityContainer container, ref PipelineContext parent)
            {
                _container = parent.Container;
                unsafe { _context = new IntPtr(Unsafe.AsPointer(ref parent)); }
                parent.Container = container;
            }

            #endregion

            public void Dispose()
            {
                unsafe
                {
                    Unsafe.AsRef<PipelineContext>(_context.ToPointer())
                          .Container = _container;
                }
            }
        }
    }
}
