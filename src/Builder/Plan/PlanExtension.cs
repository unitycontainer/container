using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Builder
{
    public static class PlanExtension
    {
        public static object ExecutePlan(this BuilderStrategy[] chain, ref BuilderContext context)
        {
            var i = -1;

            try
            {
                while (!context.BuildComplete && ++i < chain.Length)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception ex)
            {
                // TODO: 5.9.0 Add proper error message
                context.RequiresRecovery?.Recover();
                throw new ResolutionFailedException(context.OriginalBuildKey.Type,
                    context.OriginalBuildKey.Name,
                    "", ex);
            }

            return context.Existing;
        }
    }
}
