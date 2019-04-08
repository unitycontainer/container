using System;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;

namespace Unity.Strategies
{
    /// <summary>
    /// An <see cref="BuilderStrategy"/> implementation that uses
    /// a <see cref="LifetimeManager"/> to figure out if an object
    /// has already been created and to update or remove that
    /// object from some backing store.
    /// </summary>
    public class LifetimeStrategy : BuilderStrategy
    {
        #region Fields

        private readonly object _genericLifetimeManagerLock = new object();

        #endregion


        #region Build

        public override void PreBuildUp(ref BuilderContext context)
        {
            LifetimeManager policy = null;

            if (context.Registration is InternalRegistration registration)
                policy = registration.LifetimeManager;

            if (null == policy || policy is PerResolveLifetimeManager)
                policy = (LifetimeManager)context.Get(typeof(LifetimeManager));

            if (null == policy) return;

            if (policy is SynchronizedLifetimeManager recoveryPolicy)
                context.RequiresRecovery = recoveryPolicy;

            var existing = policy.GetValue(context.Lifetime);
            if (LifetimeManager.NoValue != existing)
            {
                context.Existing = existing;
                context.BuildComplete = true;
            }
        }

        public override void PostBuildUp(ref BuilderContext context)
        {
            LifetimeManager policy = null;

            if (context.Registration is InternalRegistration registration)
                policy = registration.LifetimeManager;

            if (null == policy || policy is PerResolveLifetimeManager)
                policy = (LifetimeManager)context.Get(typeof(LifetimeManager));

            policy?.SetValue(context.Existing, context.Lifetime);
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, Type type, InternalRegistration registration, params InjectionMember[] injectionMembers)
        {
            var policy = registration.LifetimeManager;
            if (null != policy)
            {
                return policy is TransientLifetimeManager ? false : true;
            }

            // Dynamic registration
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (!(registration is ContainerRegistration) && null != type && type.GetTypeInfo().IsGenericType)
                return true;
#else
            if (!(registration is ContainerRegistration) && null != type && type.IsGenericType)
                return true;
#endif
            return false;
        }

        public override bool RequiredToResolveInstance(IUnityContainer container, InternalRegistration registration) => true;

        #endregion
    }
}
