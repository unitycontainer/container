// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Strategies
{
    /// <summary>
    /// An <see cref="IBuilderStrategy"/> implementation that uses
    /// a <see cref="ILifetimePolicy"/> to figure out if an object
    /// has already been created and to update or remove that
    /// object from some backing store.
    /// </summary>
    public class LifetimeStrategy : BuilderStrategy
    {
        private readonly object _genericLifetimeManagerLock = new object();

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override object PreBuildUp(IBuilderContext context)
        {
            if (null != context.Existing) return null;

            var lifetimePolicy = GetLifetimePolicy(context, out _);
            if (null == lifetimePolicy) return null;

            if (lifetimePolicy is IRequiresRecovery recovery)
            {
                context.RecoveryStack.Add(recovery);
            }

            var existing = lifetimePolicy.GetValue(context.Lifetime);
            if (existing != null)
            {
                context.Existing = existing;
                context.BuildComplete = true;
            }

            return lifetimePolicy;
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        /// <param name="lifetimePolicy"></param>
        public override void PostBuildUp(IBuilderContext context, object lifetimePolicy = null)
        {
            (lifetimePolicy as ILifetimePolicy)?.SetValue(context.Existing, context.Lifetime);
        }

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
    }
}
