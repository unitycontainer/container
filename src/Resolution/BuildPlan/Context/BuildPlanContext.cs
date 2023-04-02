using System.Diagnostics;
using Unity.Injection;

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
        private readonly IntPtr _request;
        private readonly IntPtr _registration;

        private Type? _type;
        private Type? _generic;
        private object? _target;
        private IntPtr _contract;
        private bool _perResolve;
        private InjectionMember? _policies;
        private RegistrationManager? _manager;

        #endregion


        #region Properties

        public UnityContainer Container { get; private set; }

        public TTarget? Target { get; set; }

        public Type TargetType { get; set; }

        public RegistrationManager? Registration { get; set; }

        public bool IsFaulted { get; set; }

        #endregion
    }
}
