using Unity.Injection;
using Unity.Policy;

namespace Unity.Builder.Strategy
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown.
    /// </summary>
    public abstract class BuilderStrategy
    {

        #region Build

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        public virtual void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public virtual void PostBuildUp<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
        }

        #endregion


        #region Registration and Analysis

        /// <summary>
        /// Analyze registered type
        /// </summary>
        /// <param name="container">Reference to hosting container</param>
        /// <param name="registration">Reference to registration</param>
        /// <returns>Returns true if this strategy will participate in building of registered type</returns>
        public virtual bool RequiredToBuildType(IUnityContainer container, INamedType registration, params InjectionMember[] injectionMembers)
        {
            return true;
        }

        /// <summary>
        /// Analyses registered type
        /// </summary>
        /// <param name="container">Reference to hositng container</param>
        /// <param name="registration">Reference to registration</param>
        /// <returns>Returns true if this strategy will participate in building of registered type</returns>
        public virtual bool RequiredToResolveInstance(IUnityContainer container, INamedType registration)
        {
            return false;
        }

        #endregion
    }
}
