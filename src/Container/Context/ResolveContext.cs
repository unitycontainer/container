using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Resolution;

namespace Unity.Container
{
#if NETSTANDARD1_6 || NETCOREAPP1_0 || NETSTANDARD2_0 || NETSTANDARD2_1
    public partial class ResolveContext
#else
    public partial struct ResolveContext
#endif
    {
        public readonly Contract Contract;
        public readonly UnityContainer Container;
        public readonly RegistrationManager? Manager;
        public readonly ResolverOverride[] Overrides;

        public object? Activity;
        public object? Existing;


        #region Constructors

        public ResolveContext(UnityContainer container, in Contract contract, ResolverOverride[] overrides)
        {
            Manager   = null;
            Contract  = contract;
            Container = container;
            Overrides = overrides;

            Existing = default;
            Activity = default;
            _parent  = default;
        }

        public ResolveContext(UnityContainer container, in Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            Manager   = manager;
            Contract  = contract;
            Container = container;
            Overrides = overrides;
            
            Existing = default;
            Activity = default;
            _parent  = default;
        }

        #endregion


        #region Public API


        //public object? Existing { get; set; }

        public ICollection<IDisposable> Disposables => Container._scope.Disposables;


        #endregion
    }
}
