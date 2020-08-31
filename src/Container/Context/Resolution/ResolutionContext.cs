using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct ResolutionContext
    {
        #region Fields

        private readonly IntPtr _buildContext;
        private readonly IntPtr _contract;

        public readonly UnityContainer       Container;
        public readonly RegistrationManager  Manager;
        public readonly ResolverOverride[]   Overrides;

        public object? Existing;

        #endregion


        #region Constructors

        public ResolutionContext(UnityContainer container, ref Contract contract, ResolverOverride[] overrides)
        {
            unsafe
            {
                _buildContext = IntPtr.Zero;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Manager   = NoRegistration;
            Container = container;
            Overrides = overrides;

            Existing = default;
        }

        public ResolutionContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            unsafe
            {
                _buildContext = IntPtr.Zero;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Manager   = manager;
            Container = container;
            Overrides = overrides;

            Existing = default;
        }

        internal ResolutionContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref BuildContext parent)
        {
            unsafe
            {
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
                _buildContext = new IntPtr(Unsafe.AsPointer(ref parent));
            }

            Container = container;
            Manager   = manager;
            Overrides = parent.ResolutionContext.Overrides;

            Existing = default;
        }

        #endregion


        #region Contract
        
        public readonly ref Contract Contract
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<Contract>(_contract.ToPointer());
                }
            }
        }

        #endregion


        #region Parent

        internal readonly bool IsChild => IntPtr.Zero != _buildContext;

        internal readonly ref BuildContext Parent
        {
            get 
            {
                unsafe
                {
                    return ref Unsafe.AsRef<BuildContext>(_buildContext.ToPointer());
                }
            }
        }

        #endregion
    }
}
