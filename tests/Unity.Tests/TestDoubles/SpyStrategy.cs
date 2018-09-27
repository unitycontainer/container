using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Policy;

namespace Unity.Tests.v5.TestDoubles
{
    internal class SpyStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            this.BuildUpWasCalled = true;
            this.Context = context;
            this.BuildKey = context.BuildKey;
            this.Existing = context.Existing;

            this.UpdateSpyPolicy(context);
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            this.Existing = context.Existing;
        }

        public IBuilderContext Context { get; private set; }

        public INamedType BuildKey { get; private set; }

        public object Existing { get; private set; }

        public bool BuildUpWasCalled { get; private set; }

        private void UpdateSpyPolicy(IBuilderContext context)
        {
            var policy = (SpyPolicy)context
                .Policies
                .GetOrDefault(typeof(SpyPolicy), context.BuildKey, out _);

            if (policy != null)
            {
                policy.WasSpiedOn = true;
            }
        }
    }
}
