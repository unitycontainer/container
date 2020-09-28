using System;
using System.Diagnostics;

namespace Unity.Container
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {Type},  Name: {Name}")]
    public partial struct PipelineContext
    {
        #region Fields

        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _contract;

        public readonly UnityContainer Container;
        public readonly RegistrationManager? Registration;

        #endregion


        #region Public Properties

        public object? Action { get; set; }

        public object? Target { get; set; }

        #endregion
    }
}
