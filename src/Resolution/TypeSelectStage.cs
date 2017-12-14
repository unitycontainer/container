namespace Unity.Resolution
{
    public enum TypeSelectStage
    {
        /// <summary>
        /// First stage. By default, nothing happens here.
        /// </summary>
        Setup,

        /// <summary>
        /// Checks for already registered types
        /// </summary>
        Registration,

        /// <summary>
        /// Checks if type is array
        /// </summary>
        Array,

        /// <summary>
        /// Checks if type is generic
        /// </summary>
        Generic,

        /// <summary>
        /// Checks if type is Plain old clr object
        /// </summary>
        Poco,

        /// <summary>
        /// Throws an exception if pype could not be found
        /// </summary>
        Exception
    }
}
