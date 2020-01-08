using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext
        {
            #region Pipeline Caches

            internal Pipeline[] TypePipelineCache { get; private set; }
            internal Pipeline[] FactoryPipelineCache { get; private set; }
            internal Pipeline[] InstancePipelineCache { get; private set; }

            #endregion


            #region Pipeline Factories

            public Pipeline.PipelineBuilderDelegate TypePipelineGenerator { get; private set; }

            public ResolveDelegate<PipelineContext> FactoryPipelineGenerator { get; private set; }

            public ResolveDelegate<PipelineContext> InstancePipelineGenerator { get; private set; }

            #endregion


            #region Factories Update

            private void OnInstancePipelineChanged(object sender, EventArgs e)
            {
                TypePipelineCache = TypePipeline.ToArray();
                TypePipelineGenerator = Pipeline.ExpressFactory(TypePipelineCache);
            }

            private static bool SelectPre(Pipeline pipeline)
            {
                var method = pipeline.GetType().GetMethod("BuildExpression");

                if (typeof(Pipeline) == method.DeclaringType) return false;

                return true;
            }

            private void OnFactoryPipelineChanged(object sender, EventArgs e)
            {
                FactoryPipelineCache = FactoryPipeline.ToArray();
            }

            private void OnTypePipelineChanged(object sender, EventArgs e)
            {
                InstancePipelineCache = InstancePipeline.ToArray();
            }

            #endregion
        }
    }
}
