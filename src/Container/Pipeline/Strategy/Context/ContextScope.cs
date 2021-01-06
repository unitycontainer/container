using System;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.Extension
{
    public ref struct ContextScope
    {
        #region Fields

        private readonly IntPtr _context;

        #endregion


        #region Scope

        public ContextScope(ref BuilderContext parent)
        {
            unsafe 
            { 
                _context = new IntPtr(Unsafe.AsPointer(ref parent)); 
            }
        }

        #endregion


        public void Dispose()
        {
            unsafe
            {
            }
        }
    }
}
