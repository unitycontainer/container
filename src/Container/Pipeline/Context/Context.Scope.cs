using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct BuilderContext
    {

        public ref struct Scope
        {
            #region Fields

            private readonly IntPtr _context;
            private readonly UnityContainer _container;

            #endregion


            #region Scope

            public Scope(UnityContainer container, ref BuilderContext parent)
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
                    Unsafe.AsRef<BuilderContext>(_context.ToPointer())
                          .Container = _container;
                }
            }
        }
    }
}
