using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public static class SingletonPipelineFactory

    {
        #region Fields

        public static MethodInfo PipelineInfo   = typeof(SingletonPipelineFactory).GetMethod(nameof(BuildUpPipeline))!;
        public static MethodInfo DiagnosticInfo = typeof(SingletonPipelineFactory).GetMethod(nameof(DiagnosticPipeline))!;

        #endregion

        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;
            var processors = ((StagedChain<BuildStage, PipelineProcessor>)context.TypePipelineChain).ToArray();

            // Default activating pipelines
            policies.Set(typeof(Defaults.TypeCategory), typeof(Pipeline), PipelineInfo.CreateDelegate(typeof(Pipeline), processors));
        }


        #region Pipelines

        public static object? BuildUpPipeline(PipelineProcessor[] chain, ref ResolutionContext context)
        {
            Debug.Assert(null != context.Manager);

            PipelineContext pipeline = new PipelineContext(ref context);

            try
            {
                var i = -1;

                while (++i < chain.Length)
                {
                    chain[i].PreBuildUp(ref pipeline);

                    //if (pipeline.IsFaulted) return new ValueTask<object?>(pipeline.Exception);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref pipeline);

                    //if (pipeline.IsFaulted) return new ValueTask<object?>(pipeline.Exception);
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

            PipelineContext pipeline = new PipelineContext(ref context);

            try
            {
                var i = -1;

                while (++i < chain.Length)
                {
                    chain[i].PreBuildUp(ref pipeline);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref pipeline);
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
