using System.Diagnostics;

namespace Unity.Storage
{
    /// <summary>
    /// Internal metadata structure for hash sets and lists
    /// </summary>
    [DebuggerDisplay("Position = {Position}, Reference = {Reference}")]
    public struct Metadata
    {
        public int Reference;
        public int Position;

        public Metadata(int reference, int position)
        {
            Reference = reference;
            Position = position;
        }
    }
}
