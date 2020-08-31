using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct BuildContext 
    {
        #region Constructors

        public BuildContext(ref ResolutionContext context)
        {
            unsafe
            {
                _resolutionContext = new IntPtr(Unsafe.AsPointer(ref context));
            }

            Data = context.Manager?.Data;
            Parameter = default;
            MemberInfo = default;
            ParameterName = default;
            ParameterValue = default;
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
