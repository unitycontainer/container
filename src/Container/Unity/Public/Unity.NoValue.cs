using System;
using System.Runtime.CompilerServices;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// This is a container wide <see cref="InvalidValue"/> singleton.
        /// </summary>
        public static readonly object NoValue = new InvalidValue();

        internal static object? DummyPipeline<TContext>(ref TContext _)
            => NoValue;


        /// <summary>
        /// This is a <see cref="Type"/> of container wide <see cref="NoValue"/> singleton.
        /// </summary>
        public sealed class InvalidValue
        {
            internal InvalidValue() { }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object? obj) => ReferenceEquals(this, obj);

            public override int GetHashCode() => 0x55555555;

            public override string ToString() => "Invalid object singleton";
        }
    }
}
