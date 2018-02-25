using System;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

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
            if (null != context.Existing) return;

            var lifetimePolicy = GetLifetimePolicy(context, out _);
            if (null == lifetimePolicy) return;

            if (lifetimePolicy is IRequiresRecovery recoveryPolicy)
                context.RequiresRecovery = recoveryPolicy;

            var existing = lifetimePolicy.GetValue(context.Lifetime);
            if (existing != null)
            {
                context.Existing = existing;
                context.BuildComplete = true;
            }
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

        public override bool RegisterType(IUnityContainer container, INamedType registration, params InjectionMember[] injectionMembers)
        {
            if (registration is IPolicySet set)
            {
                var policy = set.Get(typeof(ILifetimePolicy));
                if (policy is TransientLifetimeManager)
                    return false;
            }

            return true;
        }

        public override bool RegisterInstance(IUnityContainer container, INamedType registration)
        {
            return true;
        }

        #endregion


        #region Implementation

        private ILifetimePolicy GetLifetimePolicy(IBuilderContext context, out IPolicyList source)
        {
            var policy = context.Policies.Get(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, typeof(ILifetimePolicy), out source);
            if (policy == null && context.OriginalBuildKey.Type.GetTypeInfo().IsGenericType)
            {
                policy = context.Policies.Get(context.BuildKey.Type.GetGenericTypeDefinition(), context.BuildKey.Name, typeof(ILifetimePolicy), out source);
                if (!(policy is ILifetimeFactoryPolicy factoryPolicy)) return null;

                lock (_genericLifetimeManagerLock)
                {
                    // check whether the policy for closed-generic has been added since first checked
                    var newLifetime = (ILifetimePolicy)source.Get(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, typeof(ILifetimePolicy), out _);
                    if (null == newLifetime)
                    {
                        newLifetime = factoryPolicy.CreateLifetimePolicy();
                        source.Set(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, typeof(ILifetimePolicy), newLifetime);
                        if (newLifetime is IDisposable)
                        {
                            context.Lifetime.Add(newLifetime);
                        }
                    }

                    return newLifetime;
                }
            }

            return (ILifetimePolicy)policy;
        }

        #endregion
    }
}
