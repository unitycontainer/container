using System;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
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
            LifetimeManager policy = (LifetimeManager)context.Policies.Get(context.OriginalBuildKey.Type, 
                                                                  context.OriginalBuildKey.Name, 
                                                                  typeof(LifetimeManager));
            if (null == policy)
            {
                if (context.OriginalBuildKey.Type.GetTypeInfo().IsGenericType)
                {
                    // TODO: Switch to Factory
                    policy = (LifetimeManager)context.Policies.Get(context.Type.GetGenericTypeDefinition(),
                                                                   context.Name, 
                                                                   typeof(LifetimeManager));
                    if (policy is LifetimeManager lifetimeManager)
                    {
                        lock (_genericLifetimeManagerLock)
                        {
                            // check whether the policy for closed-generic has been added since first checked
                            policy = (LifetimeManager)context.Registration.Get(typeof(LifetimeManager));
                            if (null == policy)
                            {
                                policy = lifetimeManager.CreateLifetimePolicy();
                                context.Registration.Set(typeof(LifetimeManager), policy);

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
            var lifetimePolicy = (LifetimeManager)context.Policies.Get(context.OriginalBuildKey.Type, 
                                                                       context.OriginalBuildKey.Name, 
                                                                       typeof(LifetimeManager));
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
