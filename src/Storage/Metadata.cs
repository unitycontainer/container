namespace Unity.Storage
{

    /// <summary>
    /// Internal metadata structure for hash sets and lists
    /// </summary>
    public struct Metadata
    {
        /// <summary>
        /// Next index
        /// </summary>
        public int Next;

        /// <summary>
        /// Bucket index
        /// </summary>
        public int Bucket;
    }
}
