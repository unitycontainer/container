using System.ComponentModel;
using Unity.Builder;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown.
    /// </summary>
    
    // TODO: Add url to error message
    // [Obsolete("BuilderStrategy has been replaced with use of delegates", false)]
    public abstract partial class BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        public virtual void PreBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public virtual void PostBuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
        }

        #region v4

        /// <summary>
        /// Obsolete PreTearDown. No longer supported
        /// </summary>
        [Obsolete("PreTearDown method is deprecated. Tear down no longer supported", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PreTearDown(IBuilderContext context)
        {
        }

        /// <summary>
        /// Obsolete PostTearDown. No longer supported
        /// </summary>
        [Obsolete("PostTearDown method is deprecated. Tear down no longer supported", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PostTearDown(IBuilderContext context)
        {
        }

        #endregion


        #region v5

        /// <summary>
        /// Obsolete RequiredToBuildType. No longer supported
        /// </summary>
        [Obsolete("RequiredToBuildType method is deprecated", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool RequiredToBuildType(IUnityContainer container, Type type)
        {
            return true;
        }


        /// <summary>
        /// Obsolete RequiredToResolveInstance. No longer supported
        /// </summary>
        [Obsolete("RequiredToResolveInstance method is deprecated", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool RequiredToResolveInstance(IUnityContainer container)
        {
            return false;
        }

        #endregion
    }
}
