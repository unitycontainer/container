// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Container;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.Strategy;
using Unity.Utility;

namespace Unity.ObjectBuilder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    public class BuilderContext : IBuilderContext
    {
        private readonly IStrategyChain _chain;
        private CompositeResolverOverride _resolverOverrides;   // TODO: This does not need to be List
        private bool _ownsOverrides;

        public BuilderContext(UnityContainer container, NamedTypeBuildKey key, object existing, params ResolverOverride[] resolverOverrides)
        {
            Container = container;
            OriginalBuildKey = BuildKey = key;
            Existing = existing;
            _ownsOverrides = true;            
            _resolverOverrides = new CompositeResolverOverride(resolverOverrides);
        }


        /// <summary>
        /// Initialize a new instance of the <see cref="BuilderContext"/> class with a <see cref="IStrategyChain"/>, 
        /// <see cref="ILifetimeContainer"/>, <see cref="IPolicyList"/> and the 
        /// build key used to start this build operation. 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="chain">The <see cref="IStrategyChain"/> to use for this context.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this context.</param>
        /// <param name="policies">The <see cref="IPolicyList"/> to use for this context.</param>
        /// <param name="originalBuildKey">Build key to start building.</param>
        /// <param name="existing">The existing object to build up.</param>
        public BuilderContext(IUnityContainer container, IStrategyChain chain,
            ILifetimeContainer lifetime,
            IPolicyList policies,
            NamedTypeBuildKey originalBuildKey,
            object existing)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            _chain = chain;
            Lifetime = lifetime;
            OriginalBuildKey = originalBuildKey;
            BuildKey = originalBuildKey;
            PersistentPolicies = policies;
            Policies = new PolicyList(PersistentPolicies);
            Existing = existing;
            _resolverOverrides = new CompositeResolverOverride();
            _ownsOverrides = true;
        }

        /// <summary>
        /// Create a new <see cref="BuilderContext"/> using the explicitly provided
        /// values.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="chain">The <see cref="IStrategyChain"/> to use for this context.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this context.</param>
        /// <param name="persistentPolicies">The set of persistent policies to use for this context.</param>
        /// <param name="transientPolicies">The set of transient policies to use for this context. It is
        /// the caller's responsibility to ensure that the transient and persistent policies are properly
        /// combined.</param>
        /// <param name="buildKey">Build key for this context.</param>
        /// <param name="existing">Existing object to build up.</param>
        public BuilderContext(IUnityContainer container, IStrategyChain chain, ILifetimeContainer lifetime, IPolicyList persistentPolicies, IPolicyList transientPolicies, NamedTypeBuildKey buildKey, object existing)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            _chain = chain;
            Lifetime = lifetime;
            PersistentPolicies = persistentPolicies;
            Policies = transientPolicies;
            OriginalBuildKey = buildKey;
            BuildKey = buildKey;
            Existing = existing;
            _resolverOverrides = new CompositeResolverOverride();
            _ownsOverrides = true;
        }

        /// <summary>
        /// Create a new <see cref="BuilderContext"/> using the explicitly provided
        /// values.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="chain">The <see cref="IStrategyChain"/> to use for this context.</param>
        /// <param name="lifetime">The <see cref="ILifetimeContainer"/> to use for this context.</param>
        /// <param name="persistentPolicies">The set of persistent policies to use for this context.</param>
        /// <param name="transientPolicies">The set of transient policies to use for this context. It is
        /// the caller's responsibility to ensure that the transient and persistent policies are properly
        /// combined.</param>
        /// <param name="buildKey">Build key for this context.</param>
        /// <param name="resolverOverrides">The resolver overrides.</param>
        protected BuilderContext(IUnityContainer container, IStrategyChain chain, ILifetimeContainer lifetime, IPolicyList persistentPolicies, IPolicyList transientPolicies, NamedTypeBuildKey buildKey, CompositeResolverOverride resolverOverrides)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            _chain = chain;
            Lifetime = lifetime;
            PersistentPolicies = persistentPolicies;
            Policies = transientPolicies;
            OriginalBuildKey = buildKey;
            BuildKey = buildKey;
            Existing = null;
            _resolverOverrides = resolverOverrides;
            _ownsOverrides = false;
        }

        #region IBuilderContext

        public IUnityContainer Container { get; }

        /// <summary>
        /// Gets the head of the strategy chain.
        /// </summary>
        /// <returns>
        /// The strategy that's first in the chain; returns null if there are no
        /// strategies in the chain.
        /// </returns>
        public IStrategyChain Strategies => _chain;

        /// <summary>
        /// GetOrDefault the current build key for the current build operation.
        /// </summary>
        public NamedTypeBuildKey BuildKey { get; set; }

        /// <summary>
        /// The current object being built up or torn down.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        public object Existing { get; set; }

        /// <summary>
        /// Gets the <see cref="ILifetimeContainer"/> associated with the build.
        /// </summary>
        /// <value>
        /// The <see cref="ILifetimeContainer"/> associated with the build.
        /// </value>
        public ILifetimeContainer Lifetime { get; }

        /// <summary>
        /// Gets the original build key for the build operation.
        /// </summary>
        /// <value>
        /// The original build key for the build operation.
        /// </value>
        public NamedTypeBuildKey OriginalBuildKey { get; }

        /// <summary>
        /// The set of policies that were passed into this context.
        /// </summary>
        /// <remarks>This returns the policies passed into the context.
        /// Policies added here will remain after buildup completes.</remarks>
        /// <value>The persistent policies for the current context.</value>
        public IPolicyList PersistentPolicies { get; }

        /// <summary>
        /// Gets the policies for the current context. 
        /// </summary>
        /// <remarks>
        /// Any modifications will be transient (meaning, they will be forgotten when 
        /// the outer BuildUp for this context is finished executing).
        /// </remarks>
        /// <value>
        /// The policies for the current context.
        /// </value>
        public IPolicyList Policies { get; }

        /// <summary>
        /// Gets the collection of <see cref="IRequiresRecovery"/> objects
        /// that need to execute in event of an exception.
        /// </summary>
        public IRecoveryStack RecoveryStack { get; } = new RecoveryStack();

        /// <summary>
        /// Flag indicating if the build operation should continue.
        /// </summary>
        /// <value>true means that building should not call any more
        /// strategies, false means continue to the next strategy.</value>
        public bool BuildComplete { get; set; }

        /// <summary>
        /// An object representing what is currently being done in the
        /// build chain. Used to report back errors if there's a failure.
        /// </summary>
        public object CurrentOperation { get; set; }

        /// <summary>
        /// The build context used to resolve a dependency during the build operation represented by this context.
        /// </summary>
        public IBuilderContext ChildContext { get; private set; }

        /// <summary>
        /// Add a new set of resolver override objects to the current build operation.
        /// </summary>
        /// <param name="newOverrides"><see cref="ResolverOverride"/> objects to add.</param>
        public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
        {
            if (!_ownsOverrides)
            {
                var sharedOverrides = _resolverOverrides;
                _resolverOverrides = new CompositeResolverOverride();
                _resolverOverrides.AddRange(sharedOverrides);
                _ownsOverrides = true;
            }

            _resolverOverrides.AddRange(newOverrides);
        }

        /// <summary>
        /// GetOrDefault a <see cref="IDependencyResolverPolicy"/> object for the given <paramref name="dependencyType"/>
        /// or null if that dependency hasn't been overridden.
        /// </summary>
        /// <param name="dependencyType">Type of the dependency.</param>
        /// <returns>Resolver to use, or null if no override matches for the current operation.</returns>
        public IDependencyResolverPolicy GetOverriddenResolver(Type dependencyType)
        {
            return _resolverOverrides.GetResolver(this, dependencyType);
        }

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
        public object NewBuildUp(NamedTypeBuildKey newBuildKey, Action<IBuilderContext> childCustomizationBlock = null)
        {
            ChildContext = new BuilderContext(Container, _chain, Lifetime, PersistentPolicies, Policies, newBuildKey, _resolverOverrides);

            childCustomizationBlock?.Invoke(ChildContext);
            var policy = Policies?.Get<IResolverPolicy>(newBuildKey);
            var result = policy?.Resolve(ChildContext) ?? ChildContext.Strategies.ExecuteBuildUp(ChildContext);

            ChildContext = null;

            return result;
        }

        #endregion
    }
}
