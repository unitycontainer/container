// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Unity;
using Unity.Builder;
using Unity.Container;
using Unity.Container.Lifetime;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategy;
using Unity.Utility;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class MockBuilderContext : IBuilderContext
    {
        private ILifetimeContainer lifetime = new LifetimeContainer();
        private INamedType originalBuildKey = null;
        private IPolicyList persistentPolicies;
        private IPolicyList policies;
        private MockStrategyChain strategies = new MockStrategyChain();
        private CompositeResolverOverride resolverOverrides = new CompositeResolverOverride();

        private INamedType buildKey = null;
        private object existing = null;
        private IRecoveryStack recoveryStack = new RecoveryStack();

        public MockBuilderContext()
        {
            this.persistentPolicies = new PolicyList();
            this.policies = new PolicyList(persistentPolicies);
        }

        public ILifetimeContainer Lifetime
        {
            get { return lifetime; }
        }

        public INamedType OriginalBuildKey
        {
            get { return originalBuildKey; }
        }

        public IPolicyList PersistentPolicies
        {
            get { return persistentPolicies; }
        }

        public IPolicyList Policies
        {
            get { return policies; }
        }

        public IRecoveryStack RecoveryStack
        {
            get { return recoveryStack; }
        }

        public MockStrategyChain Strategies
        {
            get { return strategies; }
        }

        IStrategyChain IBuilderContext.Strategies
        {
            get { return strategies; }
        }

        public INamedType BuildKey
        {
            get { return buildKey; }
            set
            {
                originalBuildKey = value;
                buildKey = value;
            }
        }

        public object Existing
        {
            get { return existing; }
            set { existing = value; }
        }

        public bool BuildComplete { get; set; }

        public object CurrentOperation { get; set; }

        public IBuilderContext ChildContext { get; set; }

        public IUnityContainer Container { get; set; } = new UnityContainer();

        public IBuilderContext ParentContext => throw new NotImplementedException();

        public IRequiresRecovery RequiresRecovery { get; set; }

        public void AddResolverOverrides(IEnumerable<ResolverOverride> newOverrides)
        {
            resolverOverrides.AddRange(newOverrides);
        }

        public IResolverPolicy GetOverriddenResolver(Type dependencyType)
        {
            return resolverOverrides.GetResolver(this, dependencyType);
        }

        public IBuilderContext CloneForNewBuild(INamedType newBuildKey, object newExistingObject)
        {
            var newContext = new MockBuilderContext
                                 {
                                     strategies = strategies,
                                     persistentPolicies = persistentPolicies,
                                     policies = policies,
                                     lifetime = lifetime,
                                     originalBuildKey = newBuildKey,
                                     buildKey = newBuildKey,
                                     existing = newExistingObject
                                 };
            newContext.resolverOverrides.Add(resolverOverrides);

            return newContext;
        }


        public object ExecuteBuildUp(NamedTypeBuildKey buildKey, object existing)
        {
            this.BuildKey = buildKey;
            this.originalBuildKey = buildKey;
            this.Existing = existing;

            return Strategies.ExecuteBuildUp(this);
        }

        public object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null)
        {
            var newBuildKey = new NamedTypeBuildKey(type, name);
            if (null == childCustomizationBlock)
            {
                var clone = CloneForNewBuild(newBuildKey, null);
                return clone.Strategies.ExecuteBuildUp(clone);
            }

            var newContext = new MockBuilderContext
            {
                strategies = strategies,
                persistentPolicies = persistentPolicies,
                policies = new PolicyList(persistentPolicies),
                lifetime = lifetime,
                originalBuildKey = buildKey,
                buildKey = newBuildKey,
                existing = null
            };
            newContext.resolverOverrides.Add(resolverOverrides);

            childCustomizationBlock(newContext);

            return strategies.ExecuteBuildUp(newContext);
        }
    }
}
