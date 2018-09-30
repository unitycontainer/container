using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Resolving: {OriginalBuildKey.Type},  Name: {OriginalBuildKey.Name}")]
    public class BuilderContext : IBuilderContext
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
            TypeInfo = BuildKey.Type.GetTypeInfo();
            Policies = new Storage.PolicyList(this);

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
            TypeInfo = BuildKey.Type.GetTypeInfo();
            Registration = original.Registration;
            Policies = original.Policies;
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
            Policies = parent.Policies;
            Registration = registration;
            OriginalBuildKey = (INamedType)Registration;
            BuildKey = OriginalBuildKey;
            TypeInfo = BuildKey.Type.GetTypeInfo();
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
            Policies = parent.Policies;
            Registration = registration;
            OriginalBuildKey = registration;
            BuildKey = OriginalBuildKey;
            TypeInfo = BuildKey.Type.GetTypeInfo();
        }

        #endregion


        #region INamedType

        public Type Type => BuildKey.Type;

        public string Name => BuildKey.Name;

        #endregion


        #region IBuildContext

        public IUnityContainer Container => _container;

        public object Existing { get; set; }

        public object Resolve(Type type, string name) => NewBuildUp(type, name);

        #endregion


        #region IBuilderContext

        public TypeInfo TypeInfo { get; }

        public IStrategyChain Strategies => _chain;

        public INamedType BuildKey { get; set; }

        public ILifetimeContainer Lifetime => _container._lifetimeContainer;

        public INamedType OriginalBuildKey { get; }

        public IPolicySet Registration { get; }

        public IPolicyList Policies { get; private set; }

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
            var context = this;
            return _resolverOverrides?.GetResolver(ref context, dependencyType);
        }

        #endregion

        
        #region Build

        public object BuildUp()
        {
            var i = -1;
            var chain = ((InternalRegistration)Registration).BuildChain;
            var context = this;
            try
            {
                while (!BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception)
            {
                RequiresRecovery?.Recover();
                throw;
            }

            return Existing;
        }

        public object NewBuildUp(INamedType namedType)
        {
            InternalRegistration registration = (InternalRegistration) namedType;
            var context = new BuilderContext(this, registration);
            ChildContext = context;

            var i = -1;
            var chain = registration.BuildChain;
            try
            {
                while (!context.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            var result = context.Existing;
            ChildContext = null;

            return result;
        }

        public object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null)
        {
            var context = new BuilderContext(this, type, name);
            ChildContext = context;
            childCustomizationBlock?.Invoke(ChildContext);

            var i = -1;
            var chain = ((InternalRegistration)context.Registration).BuildChain;
            try
            {
                while (!context.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            var result = context.Existing;
            ChildContext = null;

            return result;
        }

        #endregion


        #region  : Policies

        object IPolicyList.Get(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                return _container.GetPolicy(type, name, policyInterface);

            var result = Registration.Get(policyInterface);

            return result;
        }

        void IPolicyList.Set(Type type, string name, Type policyInterface, object policy)
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

        #endregion


        #region Registration

        object Get(Type type, string name, Type policyInterface)
        {
            if (!ReferenceEquals(type, OriginalBuildKey.Type) || name != OriginalBuildKey.Name)
                return _container.GetPolicy(type, name, policyInterface);

            return Registration.Get(policyInterface);

        }

        void Set(Type type, string name, Type policyInterface, object policy)
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

        #endregion
    }
}
