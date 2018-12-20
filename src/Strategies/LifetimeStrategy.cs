using System;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

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

            if (context.Registration is ContainerRegistration registration)
                policy = registration.LifetimeManager;

            if (null == policy || policy is PerResolveLifetimeManager)
                policy = context.Get<LifetimeManager>(context.Registration.Type, 
                                                      context.Registration.Name);
            if (null == policy)
            {
                if (context.Registration.Type.GetTypeInfo().IsGenericType)
                {
                    policy = context.Get<LifetimeManager>(context.Type.GetGenericTypeDefinition(),
                                                          context.Name);
                    if (policy is LifetimeManager lifetimeManager)
                    {
                        lock (_genericLifetimeManagerLock)
                        {
                            // check whether the policy for closed-generic has been added since first checked
                            policy = (LifetimeManager)((IPolicySet)context.Registration).Get(typeof(LifetimeManager));
                            if (null == policy)
                            {
                                policy = lifetimeManager.CreateLifetimePolicy();
                                ((IPolicySet)context.Registration).Set(typeof(LifetimeManager), policy);

                                if (policy is IDisposable)
                                {
                                    context.Lifetime.Add(policy);
                                }
                            }
                        }
                    }
                    else return;
                }
                else return;
            }

            if (policy is SynchronizedLifetimeManager recoveryPolicy)
                context.RequiresRecovery = recoveryPolicy;

            var existing = policy.GetValue(context.Lifetime);
            if (existing != null)
            {
                context.Existing = existing;
                context.BuildComplete = true;
            }
        }

        public override void PostBuildUp(ref BuilderContext context)
        {
            var lifetimePolicy = context.Get<LifetimeManager>(context.Registration.Type, 
                                                              context.Registration.Name);
            lifetimePolicy?.SetValue(context.Existing, context.Lifetime);
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
            if (namedType is InternalRegistration registration)
            {
                var policy = registration.Get(typeof(LifetimeManager));
                if (null != policy)
                {
                    return policy is TransientLifetimeManager ? false : true;
                }

                // Dynamic registration
                if (!(registration is ContainerRegistration) &&
                    null != registration.Type &&
                    registration.Type.GetTypeInfo().IsGenericType)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool RequiredToResolveInstance(IUnityContainer container, INamedType registration)
        {
            return true;
        }

        #endregion
    }
}
