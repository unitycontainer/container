using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {Type.Name},  Name: {Name}")]
    public partial struct PipelineContext : IResolveContext
    {
        #region Fields

        private readonly IntPtr _error;
        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _contract;

        public UnityContainer       Container;
        public RegistrationManager? Registration;

        #endregion


        #region Public Properties

        public Type Type { get => Registration?.Type ?? Contract.Type; }

        public string? Name
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Name;
                }
            }
        }
        
        public object? Target { get; set; }

        public object? Action { get; set; }

        #endregion
    }
}
