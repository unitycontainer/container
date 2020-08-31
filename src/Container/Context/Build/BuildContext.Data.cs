using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct BuildContext 
    {
        #region Fields

        private readonly IntPtr _resolutionContext;

        public object? Data;
        public object? MemberInfo;
        public string? ParameterName;
        public object? ParameterValue;
        public ParameterInfo? Parameter;

        #endregion


        #region Resolution Contexts

        public readonly ref Contract Contract
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<ResolutionContext>(_resolutionContext.ToPointer()).Contract;
                }
            }
        }

        public RegistrationManager? Manager
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ResolutionContext>(_resolutionContext.ToPointer()).Manager;
                }
            }
        }

        public object? Existing
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ResolutionContext>(_resolutionContext.ToPointer()).Existing;
                }
            }
            set
            {
                unsafe
                {
                    Unsafe.AsRef<ResolutionContext>(_resolutionContext.ToPointer()).Existing = value;
                }
            }
        }

        #endregion
    }
}
