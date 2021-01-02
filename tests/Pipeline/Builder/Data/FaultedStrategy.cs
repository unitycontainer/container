using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;

namespace Pipeline
{
    public class FaultedStrategy : BuilderStrategy
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            context.Error("Error");
        }

        public override void PostBuildUp<TContext>(ref TContext context)
        {
            Assert.Fail();
        }
    }
}
