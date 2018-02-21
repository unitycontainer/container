using System;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Registration
{
    public class TypeRegistration : InternalRegistration, 
                                    IContainerRegistration
    {
        #region Constructors

        public TypeRegistration(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager)
            : base(typeFrom ?? typeTo, string.IsNullOrEmpty(name) ? null : name)
        {
            MappedToType = typeTo;

            // Make sure manager is not being used already
            if (lifetimeManager.InUse)
            {
                if (lifetimeManager is ILifetimeFactoryPolicy factory)
                    lifetimeManager = (LifetimeManager)factory.CreateLifetimePolicy();
                else
                    throw new InvalidOperationException(Constants.LifetimeManagerInUse);
            }

            LifetimeManager = lifetimeManager;
            LifetimeManager.InUse = true;
        }

        #endregion


        #region IContainerRegistration

        public Type RegisteredType => Type;

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="InternalRegistration.Type"/> property and this one will have the same value.
        /// </summary>
        public Type MappedToType { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; }

        #endregion


        #region IPolicySet

        public override IBuilderPolicy Get(Type policyInterface)
        {
            if (typeof(ILifetimePolicy) == policyInterface)
                return LifetimeManager;

            return base.Get(policyInterface);
        }


        #endregion
    }
}
