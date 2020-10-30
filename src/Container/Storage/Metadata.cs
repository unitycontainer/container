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


    public struct MetaSet
    {
        public Metadata[] Data;
        public Metadata[] Meta;

        public MetaSet(int prime = 0)
        {
            Data = new Metadata[Prime.Numbers[prime]];
            Meta = new Metadata[Prime.Numbers[prime + 1]];
        }
    }


    public static class MetadataExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(this Metadata[] data) 
            => data[0].Position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Count(this Metadata[] data, int count)
            => data[0].Position = count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Increment(this Metadata[] data) 
            => ++data[0].Position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Version(this Metadata[] data)
            => data[0].Location;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Version(this Metadata[] data, int version)
            => data[0].Location = version;

        public static void AddRecord(this Metadata[] data, int location, int position)
        {
                var index  = ++data[0].Position;
            ref var record = ref data[index];
            record.Location = location;
            record.Position = position;
        }
    }
}
