using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Container;
using Unity.Lifetime;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public static class SingletonPipelineFactory
    {
        #region Fields

        public static MethodInfo PipelineInfo   = typeof(SingletonPipelineFactory).GetMethod(nameof(Pipeline))!;
        public static MethodInfo DiagnosticInfo = typeof(SingletonPipelineFactory).GetMethod(nameof(DiagnosticPipeline))!;

        #endregion


        #region Pipelines

        public static object? Pipeline(PipelineProcessor[] chain, ref ResolutionContext context)
        {
            Debug.Assert(null != context.Manager);

            try
            {
                var i = -1;
                var buildContext = new BuildContext(ref context);

                while (++i < chain.Length)
                {
                    chain[i].PreBuildUp(ref buildContext);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref buildContext);
                }

                return context.Existing;
            }
            catch (Exception ex)
            {
                if (context.Manager is SynchronizedLifetimeManager synchronized) 
                    synchronized.Recover();
                
                // TODO: replay exception
                throw new ResolutionFailedException(context.Type, context.Name, "Error", ex);
            }
        }

        public static object? DiagnosticPipeline(PipelineProcessor[] chain, ref ResolutionContext context)
        {
            Debug.Assert(null != context.Manager);

            try
            {
                var i = -1;
                var buildContext = new BuildContext(ref context);

                while (++i < chain.Length)
                {
                    chain[i].PreBuildUp(ref buildContext);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref buildContext);
                }

                return context.Existing;
            }
            catch (System.Exception)
            {
                if (context.Manager is SynchronizedLifetimeManager synchronized)
                    synchronized.Recover();

                // TODO: replay exception
                throw;
            }
        }

        #endregion
    }
}
