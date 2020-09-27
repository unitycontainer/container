using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class PipelineProcessor
    {
        #region Constrants

        /// <summary>
        /// Default, empty pipeline implementation
        /// </summary>
        public static readonly ResolveDelegate<PipelineContext> DefaultPipeline = (ref PipelineContext context) => context.Target;


        #endregion


        #region Build Up

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuild method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        public virtual void PreBuild(ref PipelineContext context)
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuild method is called when the chain has finished the PreBuild
        /// phase and executes in reverse order from the PreBuild calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public virtual void PostBuild(ref PipelineContext context)
        {
        }

        #endregion


        #region Public Members

        public virtual ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder) => builder.Build();

        public virtual IEnumerable<Expression> Express(ref PipelineBuilder<IEnumerable<Expression>> builder) => builder.Express();

        #endregion
    }
}
