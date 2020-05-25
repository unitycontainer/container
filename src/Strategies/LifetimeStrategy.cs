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
            LifetimeManager? lifetime = null;

            if (context.Registration is ContainerRegistration registration)
                lifetime = registration.LifetimeManager;

            if (null == lifetime || lifetime is PerResolveLifetimeManager)
                lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));

            if (null == lifetime)
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                if (!context.RegistrationType.GetTypeInfo().IsGenericType) return;
#else
                if (!context.RegistrationType.IsGenericType) return;
#endif
                var manager = (LifetimeManager?)context.Get(context.Type.GetGenericTypeDefinition(),
                                                           context.Name, typeof(LifetimeManager));
                if (null == manager) return;

                lock (_genericLifetimeManagerLock)
                {
                    // check whether the policy for closed-generic has been added since first checked
                    lifetime = (LifetimeManager?)context.Registration.Get(typeof(LifetimeManager));
                    if (null == lifetime)
                    {
                        lifetime = (LifetimeManager)manager.Clone();
                        context.Registration.Set(typeof(LifetimeManager), lifetime);

                        if (lifetime is IDisposable)
                        {
                            var scope = policy is ContainerControlledLifetimeManager container
                                      ? ((UnityContainer)container.Scope)?.LifetimeContainer ?? context.Lifetime
                                      : context.Lifetime;
                            scope.Add(policy);
                        }
                    }
                }
            }

            if (lifetime is SynchronizedLifetimeManager recoveryPolicy)
                context.RequiresRecovery = recoveryPolicy;

            var existing = lifetime.GetValue(context.Lifetime);
            if (LifetimeManager.NoValue != existing)
            {
                context.Existing = existing;
                context.BuildComplete = true;
            }
            else if (null != lifetime.Scope)
            {
                context.Scope = context.Lifetime;
                context.Lifetime = ((UnityContainer)lifetime.Scope).LifetimeContainer;
            }
        }

        public override void PostBuildUp(ref BuilderContext context)
        {
            if (null != context.Scope) context.Lifetime = context.Scope;

            LifetimeManager? lifetime = null;

            if (context.Registration is ContainerRegistration registration)
                lifetime = registration.LifetimeManager;

            if (null == lifetime || lifetime is PerResolveLifetimeManager)
                lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));

            if (LifetimeManager.NoValue != context.Existing)
                lifetime?.SetValue(context.Existing, context.Lifetime);
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, Type? type, InternalRegistration registration, params InjectionMember[] injectionMembers)
        {
            if (registration.Get(typeof(LifetimeManager)) is LifetimeManager manager)
            {
                if (null != manager.Scope) return true;

                return manager is TransientLifetimeManager ? false : true;
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
