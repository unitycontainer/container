using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Specification.Diagnostic.Issues.GitHub;
using Unity.Strategies;

namespace GitHub
{
    [TestClass]
    public class Container : Unity.Specification.Diagnostic.Issues.GitHub.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation())
                                       .AddExtension(new Diagnostic())
                                       .AddExtension(new SpyExtension(new SpyStrategy(), UnityBuildStage.Initialization));
        }
    }

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

    internal class SpyStrategy : BuilderStrategy
    {
        private object existing = null;
        private bool buildUpWasCalled = false;

        public override void PreBuildUp(ref BuilderContext context)
        {
            this.buildUpWasCalled = true;
            this.existing = context.Existing;

            this.UpdateSpyPolicy(ref context);
        }

        public override void PostBuildUp(ref BuilderContext context)
        {
            this.existing = context.Existing;
        }

        public object Existing
        {
            get { return this.existing; }
        }

        public bool BuildUpWasCalled
        {
            get { return this.buildUpWasCalled; }
        }

        private void UpdateSpyPolicy(ref BuilderContext context)
        {
            SpyPolicy policy = (SpyPolicy)context.Get(null, null, typeof(SpyPolicy));

            if (policy != null)
            {
                policy.WasSpiedOn = true;
            }
        }
    }

    internal class SpyPolicy
    {
        private bool wasSpiedOn;

        public bool WasSpiedOn
        {
            get { return wasSpiedOn; }
            set { wasSpiedOn = value; }
        }
    }
}
