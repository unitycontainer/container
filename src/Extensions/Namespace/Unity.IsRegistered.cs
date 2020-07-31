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
        /// <summary>
        /// Check if a particular type has been registered with the container with
        /// the default name.
        /// </summary>
        /// <param name="container">Container to inspect.</param>
        /// <param name="typeToCheck">Type to check registration for.</param>
        /// <returns>True if this type has been registered, false if not.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered(this IUnityContainer container, Type typeToCheck)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .IsRegistered(typeToCheck ?? throw new ArgumentNullException(nameof(typeToCheck)), null);
        }

        /// <summary>
        /// Check if a particular type has been registered with the container with the default name.
        /// </summary>
        /// <typeparam name="T">Type to check registration for.</typeparam>
        /// <param name="container">Container to inspect.</param>
        /// <returns>True if this type has been registered, false if not.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered<T>(this IUnityContainer container)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).IsRegistered(typeof(T), null);
        }

        /// <summary>
        /// Check if a particular type/name pair has been registered with the container.
        /// </summary>
        /// <typeparam name="T">Type to check registration for.</typeparam>
        /// <param name="container">Container to inspect.</param>
        /// <param name="nameToCheck">Name to check registration for.</param>
        /// <returns>True if this type/name pair has been registered, false if not.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered<T>(this IUnityContainer container, string nameToCheck)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).IsRegistered(typeof(T), nameToCheck);
        }
    }
}
