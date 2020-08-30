using System;
using System.Runtime.CompilerServices;

namespace Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the
    /// <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static partial class UnityNamespaceExtensions
    {
        #region Child Container

        /// <summary>
        /// Create a child container with no name and default capacity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer CreateChildContainer(this IUnityContainer container) 
            => (container ?? throw new ArgumentNullException(nameof(container)))
                .CreateChildContainer(null, 5);

        /// <summary>
        /// Create a child container with default capacity.
        /// </summary>
        /// <param name="name">Name of the child container</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer CreateChildContainer(this IUnityContainer container, string? name) 
            => (container ?? throw new ArgumentNullException(nameof(container)))
                .CreateChildContainer(name, 5);

        /// <summary>
        /// Create a child container with no name
        /// </summary>
        /// <param name="capacity">Preallocated capacity of child container</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer CreateChildContainer(this IUnityContainer container, int capacity) 
            => (container ?? throw new ArgumentNullException(nameof(container)))
                .CreateChildContainer(null, capacity);


        /// <summary>
        /// Create a child container with no name and default capacity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainerAsync CreateChildContainer(this IUnityContainerAsync container)
            => (container ?? throw new ArgumentNullException(nameof(container)))
                .CreateChildContainer(null, 5);

        /// <summary>
        /// Create a child container with default capacity.
        /// </summary>
        /// <param name="name">Name of the child container</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainerAsync CreateChildContainer(this IUnityContainerAsync container, string? name)
            => (container ?? throw new ArgumentNullException(nameof(container)))
                .CreateChildContainer(name, 5);

        /// <summary>
        /// Create a child container with no name
        /// </summary>
        /// <param name="capacity">Preallocated capacity of child container</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainerAsync CreateChildContainer(this IUnityContainerAsync container, int capacity)
            => (container ?? throw new ArgumentNullException(nameof(container)))
                .CreateChildContainer(null, capacity);

        #endregion
    }
}
