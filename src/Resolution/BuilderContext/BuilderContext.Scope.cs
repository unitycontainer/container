using System.Runtime.CompilerServices;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        public ref struct ContainerScope
        {
            #region Fields

            private readonly IntPtr _context;
            private readonly UnityContainer _value;

            #endregion


            #region Constructors

            internal ContainerScope(UnityContainer container, ref BuilderContext parent)
            {
                _value = parent.Container;
                unsafe { _context = new IntPtr(Unsafe.AsPointer(ref parent)); }
                parent.Container = container;
            }

            #endregion


            public void Dispose()
            {
                unsafe
                {
                    Unsafe.AsRef<BuilderContext>(_context.ToPointer())
                          .Container = _value;
                }
            }
        }
    }
}
