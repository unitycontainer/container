using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public struct BuildContext 
    {
        #region Fields
        
        private readonly IntPtr _resolutionContext;

        #endregion


        #region Constructors

        public BuildContext(ref ResolutionContext context)
        {
            unsafe
            {
                _resolutionContext = new IntPtr(Unsafe.AsPointer(ref context));
            }
        }

        #endregion


        #region ResolutionContext

        public readonly ref ResolutionContext ResolutionContext
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<ResolutionContext>(_resolutionContext.ToPointer());
                }
            }
        }

        #endregion


        #region Parent
        #endregion
    }
}
