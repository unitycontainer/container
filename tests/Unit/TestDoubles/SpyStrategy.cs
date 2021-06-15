using Unity.Builder;
using Unity.Strategies;

namespace Unity.Tests.v5.TestDoubles
{
    /// <summary>
    /// A small snoop strategy that lets us check afterwards to
    /// see if it ran in the strategy chain.
    /// </summary>
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
}
