
namespace Unity
{
    /// <summary>
    /// Enumeration to represent the object composition stages.
    /// </summary>
    /// <remarks>
    /// <para>The order of the values in the enumeration is the order in which the stages are run.</para>
    /// </remarks>
    public enum SelectionStage
    {
        /// <summary>
        /// By default, nothing happens here.
        /// </summary>
        Setup,

        /// <summary>
        /// Verification and diagnostic step.
        /// </summary>
        Diagnostic,

        /// <summary>
        /// This step allows inserting custom selection algorithms.
        /// </summary>
        Selection,

        /// <summary>
        /// Default selection algorithm.
        /// </summary>
        Default,

        /// <summary>
        /// No selection is made.
        /// </summary>
        Error
    }
}
