using System;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace Unity.Tests.v5.TestDoubles
{
    /// <summary>
    /// A simple extension that puts the supplied strategy into the
    /// chain at the indicated stage.
    /// </summary>
    internal class SpyExtension : UnityContainerExtension
    {
        private BuilderStrategy strategy;
        private UnityBuildStage stage;
        private object policy;
        private Type policyType;

        public SpyExtension(BuilderStrategy strategy, UnityBuildStage stage)
        {
            this.strategy = strategy;
            this.stage = stage;
        }

        public SpyExtension(BuilderStrategy strategy, UnityBuildStage stage, object policy, Type policyType)
        {
            this.strategy = strategy;
            this.stage = stage;
            this.policy = policy;
            this.policyType = policyType;
        }

        protected override void Initialize()
        {
            Context.Strategies.Add(this.strategy, this.stage);

            if (this.policy != null)
            {
                Context.Policies.Set(null, null, this.policyType, this.policy);
            }
        }
    }
}
