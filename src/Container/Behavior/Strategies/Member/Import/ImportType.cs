namespace Unity.Container
{
    /// <summary>
    /// Enumeration describing different types of stored data
    /// </summary>
    public enum ImportType
    {
        /// <summary>
        /// No data
        /// </summary>
        None = 0,

        /// <summary>
        /// The data is a value
        /// </summary>
        Value,

        /// <summary>
        /// The data is a pipeline
        /// </summary>
        Pipeline,

        /// <summary>
        /// The data contains analytic info about build
        /// </summary>
        Analytics,

        /// <summary>
        /// Unknown data, must be analyzed at runtime
        /// </summary>
        Dynamic
    }
}
