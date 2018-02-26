using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Strategy;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {OriginalBuildKey.Type},  Name: {OriginalBuildKey.Name}")]
    public class BuilderContext : IBuilderContext, IPolicyList
    {
        #region Fields

        private readonly IStrategyChain _chain;
        private CompositeResolverOverride _resolverOverrides;
        private bool _ownsOverrides;
        UnityContainer _container;

        #endregion

        #region Constructors

        public BuilderContext(UnityContainer container, InternalRegistration registration, 
                              object existing, params ResolverOverride[] resolverOverrides)
        {
            _container = container;
            _chain = _container._strategyChain;

            Existing = existing;
            Registration = registration;
            OriginalBuildKey = registration;
            BuildKey = OriginalBuildKey;
            Policies = new Storage.PolicyList(this);

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
            _chain = new StrategyChain(chain);
            ParentContext = original;
            OriginalBuildKey = original.OriginalBuildKey;
            BuildKey = original.BuildKey;
            Registration = original.Registration;
            Policies = original.Policies;
            Existing = existing;
            _ownsOverrides = true;
        }

        protected BuilderContext(IBuilderContext original, Type type, string name)
        {
            var parent = (BuilderContext)original;

            _container = parent._container;
            _chain = parent._chain;
            _resolverOverrides = parent._resolverOverrides;
            _ownsOverrides = false;
            ParentContext = original;
            Existing = null;
            Policies = parent.Policies;
            Registration = _container.GetRegistration(type, name);
            OriginalBuildKey = (INamedType)Registration;
            BuildKey = OriginalBuildKey;
        }

        #endregion


        #region IBuilderContext

        public IUnityContainer Container => _container;

        public IStrategyChain Strategies => _chain;

        public BuilderStrategy[] BuildChain => (OriginalBuildKey as InternalRegistration)?.BuildChain
                                                                                         ?? _chain.ToArray();
        public INamedType BuildKey { get; set; }

        public object Existing { get; set; }

        public ILifetimeContainer Lifetime => _container._lifetimeContainer;

        public INamedType OriginalBuildKey { get; }

        public IPolicySet Registration { get; }

        public IPolicyList Policies { get; }

        public IRequiresRecovery RequiresRecovery { get; set; }

        public bool BuildComplete { get; set; }

        public object CurrentOperation { get; set; }

        public IBuilderContext ChildContext { get; private set; }

        public IBuilderContext ParentContext { get; private set; }

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

        public IResolverPolicy GetOverriddenResolver(Type dependencyType)
        {
            return _resolverOverrides?.GetResolver(this, dependencyType);
        }

        #endregion

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
                return _container.GetPolicy(type, name, policyInterface, out list);

            var result = ((IPolicySet)OriginalBuildKey).Get(policyInterface);
            if (null != result) list = this;

            return result;
        }

        void IPolicyList.Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            if (type != OriginalBuildKey.Type || name != OriginalBuildKey.Name)
                _container.SetPolicy(type, name, policyInterface, policy);

            ((IPolicySet)OriginalBuildKey).Set(policyInterface, policy);
        }

        void IPolicyList.Clear(Type type, string name, Type policyInterface)
        {
            if (type != OriginalBuildKey.Type || name != OriginalBuildKey.Name)
                _container.ClearPolicy(type, name, policyInterface);

            ((IPolicySet)OriginalBuildKey).Clear(policyInterface);
        }

        void IPolicyList.ClearAll()
        {
        }

        #endregion
    }
}
