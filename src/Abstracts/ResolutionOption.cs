
namespace Unity
{
    /// <summary>
    /// Options indicating how dependency should be resolved
    /// </summary>
    public enum ResolutionOption
    {
        /// <summary>
        /// Required dependency
        /// </summary>
        /// <remarks>
        /// This dependency should be either resolved or error
        /// should be reported.
        /// </remarks>
        Required,

        /// <summary>
        /// Optional dependency
        /// </summary>
        /// <remarks>
        /// This dependency should be either resolved or default 
        /// value will be returned.
        /// </remarks>
        Optional
    }
}
