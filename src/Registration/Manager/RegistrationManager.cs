using System;
using System.ComponentModel.Composition;
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
            => Add(members);

        #endregion


        #region Policies

        public virtual ImportSource Source => ImportSource.Any;

        /// <summary>
        /// Creation policy
        /// </summary>
        public virtual CreationPolicy CreationPolicy => CreationPolicy.Any;

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
            Data         = manager;
            Other        = manager.Other;
            Fields       = manager.Fields;
            Methods      = manager.Methods;
            Category     = RegistrationCategory.Clone;
            Properties   = manager.Properties;
            Constructor  = manager.Constructor;
            RequireBuild = manager.RequireBuild;

            if (null != members && 0 != members.Length) Add(members);
        }

        #endregion
    }
}
