using System;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Registration;

namespace Unity.Strategies
{
    /// <summary>
    /// An <see cref="BuilderStrategy"/> implementation that uses
    /// a <see cref="ILifetimePolicy"/> to figure out if an object
    /// has already been created and to update or remove that
    /// object from some backing store.
    /// </summary>
    public class LifetimeStrategy : BuilderStrategy
    {
        #region Fields

        private readonly object _genericLifetimeManagerLock = new object();

        #endregion


        #region Build

        public override void PreBuildUp(IBuilderContext context)
        {
            ILifetimePolicy policy = (ILifetimePolicy)context.Policies.Get(context.OriginalBuildKey.Type, 
                                                                  context.OriginalBuildKey.Name, 
                                                                  typeof(ILifetimePolicy), out _);
            if (null == policy)
            {
                if (context.OriginalBuildKey.Type.GetTypeInfo().IsGenericType)
                {
                    policy = (ILifetimePolicy)context.Policies.Get(context.BuildKey.Type.GetGenericTypeDefinition(),
                                                          context.BuildKey.Name, typeof(ILifetimePolicy), out _);
                    if (policy is ILifetimeFactoryPolicy factoryPolicy)
                    {
                        lock (_genericLifetimeManagerLock)
                        {
                            // check whether the policy for closed-generic has been added since first checked
                            policy = (ILifetimePolicy)context.Registration.Get(typeof(ILifetimePolicy));
                            if (null == policy)
                            {
                                policy = factoryPolicy.CreateLifetimePolicy();
                                context.Registration.Set(typeof(ILifetimePolicy), policy);

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

            var existing = policy.GetValue(context.Lifetime);
            if (existing != null)
            {
                context.Existing = existing;
                context.BuildComplete = true;
                return;
            }

            if (policy is IRequiresRecovery recoveryPolicy)
                context.RequiresRecovery = recoveryPolicy;
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            var lifetimePolicy = (ILifetimePolicy)context.Policies.Get(context.OriginalBuildKey.Type, 
                                                                       context.OriginalBuildKey.Name, 
                                                                       typeof(ILifetimePolicy), out _);
            lifetimePolicy?.SetValue(context.Existing, context.Lifetime);
        }

        #endregion


        #region Registration and Analysis

        public override bool RequiredToBuildType(IUnityContainer container, INamedType namedType, params InjectionMember[] injectionMembers)
        {
            if (namedType is InternalRegistration registration)
            {
                var policy = registration.Get(typeof(ILifetimePolicy));
                if (null != policy)
                {
                    if (policy is ISingletonLifetimePolicy || policy is IContainerLifetimePolicy)
                        registration.EnableOptimization = false;

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
