using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Storage
{
    /// <summary>
    /// Internal metadata structure for hash sets and lists
    /// </summary>
    [DebuggerDisplay("Position = {Position}, Location = {Location}")]
    public struct Metadata
    {
        public int Location;
        public int Position;

        public Metadata(int location, int position)
        {
            Location = location;
            Position = position;
        }
    }

    public static class MetadataExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(this Metadata[] data)
        {
#if DEBUG
            Debug.Assert(0 < data.Length);
#endif
            return data[0].Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Increment(this Metadata[] data)
        {
#if DEBUG
            Debug.Assert(0 < data.Length);
#endif
            return ++data[0].Position;
        }
    }
}
