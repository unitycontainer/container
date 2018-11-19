using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            _ownsOverrides = true;
            if (null != resolverOverrides && 0 < resolverOverrides.Length)
            {
                _resolverOverrides = new CompositeResolverOverride(resolverOverrides);
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
            _policies = original.Policies;
            Existing = existing;
            _ownsOverrides = true;
        }

        internal BuilderContext(IBuilderContext original, InternalRegistration registration)
        {
            var parent = (BuilderContext)original;

            _container = parent._container;
            _chain = parent._chain;
            _resolverOverrides = parent._resolverOverrides;
            _ownsOverrides = false;
            ParentContext = original;
            Existing = null;
            _policies = parent.Policies;
            Registration = registration;
            OriginalBuildKey = (INamedType)Registration;
            BuildKey = OriginalBuildKey;
        }

        internal BuilderContext(IBuilderContext original, Type type, string name)
        {
            var parent = (BuilderContext)original;
            var registration = (InternalRegistration)parent._container.GetRegistration(type, name);

            _container = parent._container;
            _chain = parent._chain;
            _resolverOverrides = parent._resolverOverrides;
            _ownsOverrides = false;
            ParentContext = original;
            Existing = null;
            _policies = parent.Policies;
            Registration = registration;
            OriginalBuildKey = registration;
            BuildKey = OriginalBuildKey;
        }

        #endregion


        #region IBuilderContext

        public IUnityContainer Container => _container;

        public IStrategyChain Strategies => _chain;

        public INamedType BuildKey { get; set; }

        public object Existing { get; set; }

        public ILifetimeContainer Lifetime => _container._lifetimeContainer;

        public INamedType OriginalBuildKey { get; }

        public IPolicySet Registration { get; }

        private IPolicyList _policies;
        public IPolicyList Policies => _policies ?? (_policies = new Storage.PolicyList(this));

        public IRequiresRecovery RequiresRecovery { get; set; }

        public bool BuildComplete { get; set; }

        public object CurrentOperation { get; set; }

        public IBuilderContext ChildContext { get; internal set; }

        public IBuilderContext ParentContext { get; private set; }

        public IPolicyList PersistentPolicies => this;

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

        
        #region Build

        public object BuildUp()
        {
            var i = -1;
            var chain = ((InternalRegistration)Registration).BuildChain;

            try
            {
                while (!BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(this);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(this);
                }
            }
            catch (Exception)
            {
                RequiresRecovery?.Recover();
                throw;
            }

            return Existing;
        }

        public object NewBuildUp(InternalRegistration registration)
        {
            ChildContext = new BuilderContext(this, registration);

            var i = -1;
            var chain = registration.BuildChain;

            try
            {
                while (!ChildContext.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ChildContext);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ChildContext);
                }
            }
            catch (Exception)
            {
                ChildContext.RequiresRecovery?.Recover();
                throw;
            }

            var result = ChildContext.Existing;
            ChildContext = null;

            return result;
        }

        public object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null)
        {
            ChildContext = new BuilderContext(this, type, name);
            childCustomizationBlock?.Invoke(ChildContext);

            var i = -1;
            var chain = ((InternalRegistration)ChildContext.Registration).BuildChain;

            try
            {
                while (!ChildContext.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ChildContext);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ChildContext);
                }
            }
            catch (Exception)
            {
                ChildContext.RequiresRecovery?.Recover();
                throw;
            }

            var result = ChildContext.Existing;
            ChildContext = null;

            return result;
        }

        #endregion


        #region  : Policies

        IBuilderPolicy IPolicyList.Get(Type type, string name, Type policyInterface, out IPolicyList list)
        {
            list = null;

            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                return _container.GetPolicy(type, name, policyInterface, out list);

            var result = Registration.Get(policyInterface);
            if (null != result) list = this;

            return result;
        }

        void IPolicyList.Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            Policies.Set(type, name, policyInterface, policy);
        }

        void IPolicyList.Clear(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                _container.ClearPolicy(type, name, policyInterface);
            else
                Registration.Clear(policyInterface);
        }

        void IPolicyList.ClearAll()
        {
        }

        #endregion


        #region Registration

        IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
        {
            list = null;

            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                return _container.GetPolicy(type, name, policyInterface, out list);

            var result = Registration.Get(policyInterface);
            if (null != result) list = this;

            return result;
        }

        void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            if (type != OriginalBuildKey.Type || name != OriginalBuildKey.Name)
                _container.SetPolicy(type, name, policyInterface, policy);
            else
                Registration.Set(policyInterface, policy);
        }

        void Clear(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                _container.ClearPolicy(type, name, policyInterface);
            else
                Registration.Clear(policyInterface);
        }

        void ClearAll()
        {
        }

        #endregion
    }
}
