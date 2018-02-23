using System;
using System.Collections.Generic;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.Strategy;
using Unity.Utility;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    public class BuilderContext : IBuilderContext
    {
        #region Fields

        private readonly IStrategyChain _chain;
        private CompositeResolverOverride _resolverOverrides;
        private bool _ownsOverrides;

        #endregion


        #region Constructors

        public BuilderContext(IUnityContainer container, ILifetimeContainer lifetime, IStrategyChain chain,
            IPolicyList persistentPolicies, IPolicyList policies, INamedType buildKey, object existing, params ResolverOverride[] resolverOverrides)
        {
            _chain = chain;
            Container = container;
            Lifetime = lifetime;
            Existing = existing;

            OriginalBuildKey = buildKey;
            BuildKey = OriginalBuildKey;

            Policies = policies;
            PersistentPolicies = persistentPolicies;

            _ownsOverrides = true;
            if (null != resolverOverrides && 0 < resolverOverrides.Length)
            {
                _resolverOverrides = new CompositeResolverOverride();
                _resolverOverrides.AddRange(resolverOverrides);
            }
        }

        public BuilderContext(IBuilderContext original, IStrategyChain chain, object existing)
        {
            _chain = chain;
            Container = original.Container;
            ParentContext = original;
            Lifetime = original.Lifetime;
            OriginalBuildKey = original.OriginalBuildKey;
            BuildKey = original.BuildKey;
            PersistentPolicies = original.PersistentPolicies;
            Policies = original.Policies;
            Existing = existing;
            _ownsOverrides = true;
        }


        protected BuilderContext(IBuilderContext original, Type type, string name)
        {
            var parent = (BuilderContext)original;

            _chain = parent._chain;
            ParentContext = original;
            Container = original.Container;
            Lifetime = parent.Lifetime;
            Existing = null;
            _resolverOverrides = parent._resolverOverrides;
            _ownsOverrides = false;
            Policies = parent.Policies;
            PersistentPolicies = parent.PersistentPolicies;
            OriginalBuildKey = new NamedTypeBuildKey(type, name);
            BuildKey = OriginalBuildKey;
        }

        #endregion


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
        public INamedType BuildKey { get; set; }

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
        public INamedType OriginalBuildKey { get; }

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
        private IRecoveryStack _recoveryStack;
        public IRecoveryStack RecoveryStack
        {
            get
            {
                if (null == _recoveryStack)
                    _recoveryStack = new RecoveryStack();

                return _recoveryStack;
            }
        }

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

        public IBuilderContext ParentContext { get; private set; }

        /// <summary>
        /// Add a new set of resolver override objects to the current build operation.
        /// </summary>
        /// <param name="newOverrides"><see cref="ResolverOverride"/> objects to add.</param>
        public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
        {
            if (null == _resolverOverrides)
            {
                _resolverOverrides = new CompositeResolverOverride();
            }
            else if (!_ownsOverrides)
            {
                var sharedOverrides = _resolverOverrides;
                _resolverOverrides = new CompositeResolverOverride();
                _resolverOverrides.AddRange(sharedOverrides);
                _ownsOverrides = true;
            }

            _resolverOverrides.AddRange(newOverrides);
        }

        /// <summary>
        /// GetOrDefault a <see cref="IResolverPolicy"/> object for the given <paramref name="dependencyType"/>
        /// or null if that dependency hasn't been overridden.
        /// </summary>
        /// <param name="dependencyType">Type of the dependency.</param>
        /// <returns>Resolver to use, or null if no override matches for the current operation.</returns>
        public IResolverPolicy GetOverriddenResolver(Type dependencyType)
        {
            return _resolverOverrides?.GetResolver(this, dependencyType);
        }

        #endregion

        /// <summary>
        /// A method to do a new buildup operation on an existing context.
        /// </summary>
        /// <param name="type">Type of to build</param>
        /// <param name="name">Name of the type to build</param>
        /// <param name="childCustomizationBlock">A delegate that takes a <see cref="IBuilderContext"/>. This
        /// is invoked with the new child context before the build up process starts. This gives callers
        /// the opportunity to customize the context for the build process.</param>
        /// <returns>Resolved object</returns>
        public object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null)
        {
            ChildContext = new BuilderContext(this, type, name);

            childCustomizationBlock?.Invoke(ChildContext);
            var result = ChildContext.Strategies.ExecuteBuildUp(ChildContext);
            ChildContext = null;

            return result;
        }

    }
}
