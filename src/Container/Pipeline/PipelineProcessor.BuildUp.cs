using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public abstract partial class PipelineProcessor
    {
        #region Build Up

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        public virtual void PreBuildUp(ref ResolutionContext context)
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public virtual void PostBuildUp(ref ResolutionContext context)
        {
        }

        #endregion


        #region Diagnostic Build Up


        /// <summary>
        /// Diagnostic version of <see cref="PreBuildUp(ref ResolveContext)"/>.
        /// </summary>
        /// <remarks>
        /// This method is called instead of <see cref="PreBuildUp(ref ResolveContext)"/>
        /// when diagnostic is enabled and is being observed by listeners
        /// </remarks>
        /// <param name="context">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void PreDiagnosticBuildUp(ref ResolutionContext context)
            => PreBuildUp(ref context);

        /// <summary>
        /// Diagnostic version of <see cref="PostBuildUp(ref ResolveContext)"/>.
        /// </summary>
        /// <remarks>
        /// This method is called instead of <see cref="PreBuildUp(ref ResolveContext)"/>
        /// when diagnostic is enabled and is being observed by listeners
        /// </remarks>
        /// <param name="context">Context of the build operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void PostDiagnosticBuildUp(ref ResolutionContext context)
            => PostBuildUp(ref context);

        #endregion
    }
}
