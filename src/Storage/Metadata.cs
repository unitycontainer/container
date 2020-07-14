using System.Diagnostics;

namespace Unity.Storage
{
    /// <summary>
    /// Internal metadata structure for hash sets and lists
    /// </summary>
    [DebuggerDisplay("Position = {Position}, Next = {Next}")]
    public struct Metadata
    {
        public int Next;
        public int Position;
    }
}
