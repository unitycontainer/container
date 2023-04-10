using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Resolution
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("BuildPlanContext: Type: {TargetType?.Name}, Scope: {Container}")]
    public partial struct BuildPlanContext<TTarget> : IBuildPlanContext<TTarget>
    {
        #region Fields

        private readonly IntPtr _error;
        private readonly IntPtr _parent;
        private RegistrationManager? _manager;

        #endregion


        #region Properties

        public UnityContainer Container { get; private set; }

        public TTarget? Target { get; set; }

        public Type TargetType { get; set; }

        public RegistrationManager? Registration
        {
            get => _manager;
        }

        public bool IsFaulted 
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ErrorDescriptor>(_error.ToPointer())
                                 .IsFaulted;
                }
            }
            set
            {
                {
                    unsafe
                    {
                        Unsafe.AsRef<ErrorDescriptor>(_error.ToPointer())
                              .IsFaulted = value;
                    }
                }
            }
        }

        #endregion
    }
}
