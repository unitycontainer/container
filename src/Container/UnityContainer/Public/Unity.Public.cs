using System.Runtime.CompilerServices;

namespace Unity
{
    public sealed partial class UnityContainer
    {
        #region Constants

        /// <summary>
        /// Default name of root container
        /// </summary>
        private const string DEFAULT_ROOT_NAME = "root";

        /// <summary>
        /// Default capacity of root container
        /// </summary>
        private const int DEFAULT_ROOT_CAPACITY = 59;

        #endregion


        #region Properties

        public string? Name { get; }

        public UnityContainer Root { get; }

        public UnityContainer? Parent { get; }

        #endregion


        #region NoValue

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

        #endregion
    }
}
