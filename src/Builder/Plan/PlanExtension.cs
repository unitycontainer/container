using System;

namespace Unity.Builder
{
    public static class PlanExtension
    {
        public static object ExecutePlan(this BuilderStrategy[] chain, ref BuilderContext context)
        {
            var i = -1;

            while (!context.BuildComplete && ++i < chain.Length)
            {
                chain[i].PreBuildUp(ref context);
            }

            while (--i >= 0)
            {
                chain[i].PostBuildUp(ref context);
            }

            return context.Existing;
        }

        public static object ExecuteThrowingPlan(this BuilderStrategy[] chain, ref BuilderContext context)
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
                context.RequiresRecovery?.Recover();
                // TODO: 5.9.0 Add proper error message
                throw new ResolutionFailedException(context.Registration.Type,
                    context.Registration.Name,
                    "", ex);
            }

            return context.Existing;
        }

        public static object ExecuteReThrowingPlan(this BuilderStrategy[] chain, ref BuilderContext context)
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
            catch
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            return context.Existing;
        }
    }
}
