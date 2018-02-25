using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategy;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    public class BuilderContext : IBuilderContext, IPolicyList
    {
        #region Fields

        private readonly IPolicyList _policies;
        private readonly IStrategyChain _chain;
        private CompositeResolverOverride _resolverOverrides;
        private bool _ownsOverrides;
        UnityContainer _container;

        #endregion

        #region Constructors

        public BuilderContext(UnityContainer container, IPolicyList policies, InternalRegistration registration, 
                              object existing, params ResolverOverride[] resolverOverrides)
        {
            _container = container;
            _chain = _container._strategyChain;
            _policies = policies;

            Existing = existing;
            OriginalBuildKey = registration;
            BuildKey = OriginalBuildKey;
            PersistentPolicies = this;
            Policies = new PolicyList(PersistentPolicies);
            BuildChain = registration.BuildChain ?? _container._buildChain;

            _ownsOverrides = true;
            if (null != resolverOverrides && 0 < resolverOverrides.Length)
            {
                _resolverOverrides = new CompositeResolverOverride();
                _resolverOverrides.AddRange(resolverOverrides);
            }
        }

        public BuilderContext(IBuilderContext original, IEnumerable<BuilderStrategy> chain, object existing)
        {
            _container = ((BuilderContext)original)._container;
            _policies = ((BuilderContext)original)._policies;
            _chain = new StrategyChain(chain);
            BuildChain = chain.ToArray();
            ParentContext = original;
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

            _container = parent._container;
            _policies = parent._policies;
            _chain = parent._chain;
            _resolverOverrides = parent._resolverOverrides;
            _ownsOverrides = false;
            ParentContext = original;
            Existing = null;
            Policies = parent.Policies;
            PersistentPolicies = parent.PersistentPolicies;
            OriginalBuildKey = new NamedTypeBuildKey(type, name);
            BuildKey = OriginalBuildKey;
            BuildChain = _container._buildChain;
        }

        #endregion


        #region IBuilderContext

        public IUnityContainer Container => _container;

        /// <summary>
        /// Gets the head of the strategy chain.
        /// </summary>
        /// <returns>
        /// The strategy that's first in the chain; returns null if there are no
        /// strategies in the chain.
        /// </returns>
        public IStrategyChain Strategies => _chain;

        /// <summary>
        /// Set of strategies used for building of this context
        /// </summary>
        public BuilderStrategy[] BuildChain { get; }

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
        public ILifetimeContainer Lifetime => _container._lifetimeContainer;

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
        /// Reference to Lifetime manager which requires recovery
        /// </summary>
        public IRequiresRecovery RequiresRecovery { get; set; }

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

        #region  : IPolicyList

        IBuilderPolicy IPolicyList.Get(Type type, string name, Type policyInterface, out IPolicyList list)
        {
            list = null;

            if (type != OriginalBuildKey.Type || name != OriginalBuildKey.Name)
                return _policies.Get(type, name, policyInterface, out list);

            var result = ((IPolicySet)OriginalBuildKey).Get(policyInterface);
            if (null != result) list = this;

            return result;
        }

        void IPolicyList.Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            if (type != OriginalBuildKey.Type || name != OriginalBuildKey.Name)
                _policies.Set(type, name, policyInterface, policy);

            ((IPolicySet)OriginalBuildKey).Set(policyInterface, policy);
        }

        void IPolicyList.Clear(Type type, string name, Type policyInterface)
        {
            if (type != OriginalBuildKey.Type || name != OriginalBuildKey.Name)
                _policies.Clear(type, name, policyInterface);

            ((IPolicySet)OriginalBuildKey).Clear(policyInterface);
        }

        void IPolicyList.ClearAll()
        {
        }

        #endregion
    }
}
