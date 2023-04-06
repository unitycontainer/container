using System.Diagnostics;

namespace Unity.Resolution
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Context: Type: {Contract.Type?.Name},  Name: {Contract.Name},  Scope: {Container.Name}")]
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

        public bool IsFaulted { get; set; }

        #endregion
    }
}
