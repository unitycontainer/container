using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_V5 || UNITY_V6
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;
#else
using Unity.Extension;
#endif


namespace Regression.Container
{
    /// <summary>
    /// Implementation of <see cref="BuilderStrategy"/> which will notify an object about
    /// the completion of a BuildUp operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This strategy checks the object that is passing through the builder chain to see if it
    /// implements IBuilderAware and if it does, it will call <see cref="IBuilderAware.OnBuiltUp"/>. 
    /// </para>
    /// <para>
    /// Starting with Unity v5 this strategy is no longer part of Unity container implementation
    /// but as demonstrated in this example could be easily added to the container.
    /// </para>
    /// </remarks>
    public class BuilderAwareStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
#if UNITY_V5 || UNITY_V6
        public override void PreBuildUp(ref BuilderContext context)
        {
            if (context.Existing is IBuilderAware aware)
                aware.OnBuiltUp(context.Type, context.Name);
        }
#else
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            if (context.Existing is IBuilderAware aware)
                aware.OnBuiltUp(context.Type, context.Name);
        }
#endif
    }
}
