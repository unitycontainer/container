using System;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager
    {
        #region Constructors

        public RegistrationManager(params InjectionMember[] members)
            => Inject(members);

        #endregion


        #region Registration Data

        public bool RequireBuild { get; private set; }

        public object? Data { get; internal set; }

        public RegistrationCategory Category { get; internal set; }

        #endregion


        #region Registration Types

        public Type? Type =>
            RegistrationCategory.Type == Category
                ? (Type?)Data
                : null;

        public object? Instance =>
            RegistrationCategory.Instance == Category
                ? Data
                : UnityContainer.NoValue;

        public IUnityContainer.FactoryDelegate? Factory =>
            RegistrationCategory.Factory == Category
                ? (IUnityContainer.FactoryDelegate?)Data
                : null;

        #endregion


        #region Clone

        protected virtual void CloneData(RegistrationManager manager, InjectionMember[]? members = null)
        {
            Constructors = manager.Constructors;
            Fields       = manager.Fields;
            Properties   = manager.Properties;
            Methods      = manager.Methods;
            Other        = manager.Other;

            Data         = manager;
            Category     = RegistrationCategory.Clone;
            RequireBuild = manager.RequireBuild;

            if (null != members && 0 != members.Length) Inject(members);
        }

        #endregion
    }
}
