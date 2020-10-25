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
            => data[0].Position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Increment(this Metadata[] data) 
            => ++data[0].Position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Version(this Metadata[] data)
            => data[0].Location;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Version(this Metadata[] data, int version)
            => data[0].Location = version;
    }
}
