using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {Type},  Name: {Name}")]
    public partial struct PipelineContext : IResolveContext
    {
        #region Fields

        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _contract;

        public readonly UnityContainer Container;
        public readonly RegistrationManager? Registration;

        #endregion


        #region Public Properties

        public Type Type { get; private set; }

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
