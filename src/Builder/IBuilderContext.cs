// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.Strategy;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    public interface IBuilderContext
    {
        /// <summary>
        /// Gets Reference to container.
        /// </summary>
        /// <returns>
        /// Interface for the hosting container
        /// </returns>
        IUnityContainer Container { get; }

        /// <summary>
        /// Gets the head of the strategy chain.
        /// </summary>
        /// <returns>
        /// The strategy that's first in the chain; returns null if there are no
        /// strategies in the chain.
        /// </returns>
        IStrategyChain Strategies { get; }

        /// <summary>
        /// Gets the <see cref="ILifetimeContainer"/> associated with the build.
        /// </summary>
        /// <value>
        /// The <see cref="ILifetimeContainer"/> associated with the build.
        /// </value>
        ILifetimeContainer Lifetime { get; }

        /// <summary>
        /// Gets the original build key for the build operation.
        /// </summary>
        /// <value>
        /// The original build key for the build operation.
        /// </value>
        NamedTypeBuildKey OriginalBuildKey { get; }

        /// <summary>
        /// GetOrDefault the current build key for the current build operation.
        /// </summary>
        NamedTypeBuildKey BuildKey { get; set; }

        /// <summary>
        /// The set of policies that were passed into this context.
        /// </summary>
        /// <remarks>This returns the policies passed into the context.
        /// Policies added here will remain after buildup completes.</remarks>
        /// <value>The persistent policies for the current context.</value>
        IPolicyList PersistentPolicies { get; }

        /// <summary>
        /// Gets the policies for the current context. 
        /// </summary>
        /// <remarks>Any policies added to this object are transient
        /// and will be erased at the end of the buildup.</remarks>
        /// <value>
        /// The policies for the current context.
        /// </value>
        IPolicyList Policies { get; }

        /// <summary>
        /// Gets the collection of <see cref="IRequiresRecovery"/> objects
        /// that need to execute in event of an exception.
        /// </summary>
        IRecoveryStack RecoveryStack { get; }

        /// <summary>
        /// The current object being built up or torn down.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        object Existing { get; set; }

        /// <summary>
        /// Flag indicating if the build operation should continue.
        /// </summary>
        /// <value>true means that building should not call any more
        /// strategies, false means continue to the next strategy.</value>
        bool BuildComplete { get; set; }

        /// <summary>
        /// An object representing what is currently being done in the
        /// build chain. Used to report back errors if there's a failure.
        /// </summary>
        object CurrentOperation { get; set; }

        /// <summary>
        /// The build context used to resolve a dependency during the build operation represented by this context.
        /// </summary>
        IBuilderContext ChildContext { get; }

        /// <summary>
        /// Add a new set of resolver override objects to the current build operation.
        /// </summary>
        /// <param name="newOverrides"><see cref="ResolverOverride"/> objects to add.</param>
        void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides);

        /// <summary>
        /// GetOrDefault a <see cref="IDependencyResolverPolicy"/> object for the given <paramref name="dependencyType"/>
        /// or null if that dependency hasn't been overridden.
        /// </summary>
        /// <param name="dependencyType">Type of the dependency.</param>
        /// <returns>Resolver to use, or null if no override matches for the current operation.</returns>
        IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType);

        /// <summary>
        /// A convenience method to do a new buildup operation on an existing context. This
        /// overload allows you to specify extra policies which will be in effect for the duration
        /// of the build.
        /// </summary>
        /// <param name="newBuildKey">Key defining what to build up.</param>
        /// <param name="childCustomizationBlock">A delegate that takes a <see cref="IBuilderContext"/>. This
        /// is invoked with the new child context before the build up process starts. This gives callers
        /// the opportunity to customize the context for the build process.</param>
        /// <returns>Created object.</returns>
        object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock = null);
    }

    /// <summary>
    /// Extension methods to provide convenience overloads over the
    /// <see cref="IBuilderContext"/> interface.
    /// </summary>
    public static class BuilderContextExtensions
    {
        /// <summary>
        /// Start a recursive build up operation to retrieve the default
        /// value for the given <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type of object to build.</typeparam>
        /// <param name="context">Parent context.</param>
        /// <returns>Resulting object.</returns>
        public static TResult NewBuildUp<TResult>(this IBuilderContext context)
        {
            return context.NewBuildUp<TResult>(null);
        }

        /// <summary>
        /// Start a recursive build up operation to retrieve the named
        /// implementation for the given <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TResult">Type to resolve.</typeparam>
        /// <param name="context">Parent context.</param>
        /// <param name="name">Name to resolve with.</param>
        /// <returns>The resulting object.</returns>
        public static TResult NewBuildUp<TResult>(this IBuilderContext context, string name)
        {
            return (TResult)(context ?? throw new ArgumentNullException(nameof(context)))
                .NewBuildUp(NamedTypeBuildKey.Make<TResult>(name));
        }

        /// <summary>
        /// Add a set of <see cref="ResolverOverride"/>s to the context, specified as a 
        /// variable argument list.
        /// </summary>
        /// <param name="context">Context to add overrides to.</param>
        /// <param name="overrides">The overrides.</param>
        public static void AddResolverOverrides(this IBuilderContext context, params ResolverOverride[] overrides)
        {
            (context ?? throw new ArgumentNullException(nameof(context)))
                .AddResolverOverrides(overrides);
        }


        /// <summary>
        /// A helper method used by the generated IL to set up a PerResolveLifetimeManager lifetime manager
        /// if the current object is such.
        /// </summary>
        /// <param name="context">Current build context.</param>
        public static void SetPerBuildSingleton(this IBuilderContext context)
        {
            var lifetime = (context ?? throw new ArgumentNullException(nameof(context))).Policies.Get<ILifetimePolicy>(context.OriginalBuildKey);
            if (lifetime is PerResolveLifetimeManager)
            {
                var perBuildLifetime = new InternalPerResolveLifetimeManager(context.Existing);
                context.Policies.Set<ILifetimePolicy>(perBuildLifetime, context.OriginalBuildKey);
            }
        }

    }


    // TODO: Unify this with container
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    internal class InternalPerResolveLifetimeManager : PerResolveLifetimeManager
    {
        /// <summary>
        /// Construct a new <see cref="PerResolveLifetimeManager"/> object that stores the
        /// give value. This value will be returned by <see cref="LifetimeManager.GetValue"/>
        /// but is not stored in the lifetime manager, nor is the value disposed.
        /// This Lifetime manager is intended only for internal use, which is why the
        /// normal <see cref="LifetimeManager.SetValue"/> method is not used here.
        /// </summary>
        /// <param name="value">Value to store.</param>
        public InternalPerResolveLifetimeManager(object value)
        {
            base.value = value;
            InUse = true;
        }
    }
}
