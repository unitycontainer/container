using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public readonly struct ResolutionContext
    {
        #region Fields

        private readonly IntPtr _parent;
        public readonly string Name;

        #endregion


        #region Constructors

        public ResolutionContext(string name)
        {
            _parent = IntPtr.Zero;
            Name = name;
        }

        public ResolutionContext(ref ResolutionContext parent)
        {
            unsafe
            { 
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
            }
             
            Name = Guid.NewGuid().ToString();
        }

        #endregion


        public bool IsChild => _parent != IntPtr.Zero;

        public readonly ref ResolutionContext Parent
        {
            get 
            {
                unsafe
                {
                    return ref Unsafe.AsRef<ResolutionContext>(_parent.ToPointer());
                }
            }
        }

        public long Id => (long)_parent;

    }
}
