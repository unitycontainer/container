using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the
    /// <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static partial class UnityNamespaceExtensions
    {
        #region Resolve overloads

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of object to get from the container.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Resolve<T>(this IUnityContainer container, params ResolverOverride[] overrides)
        {
            return (T?)(container ?? throw new ArgumentNullException(nameof(container))).Resolve(typeof(T), null, overrides);
        }

        /// <summary>
        /// Resolve an instance of the requested type with the given name from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of object to get from the container.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Resolve<T>(this IUnityContainer container, string name, params ResolverOverride[] overrides)
        {
            return (T?)(container ?? throw new ArgumentNullException(nameof(container))).Resolve(typeof(T), name, overrides);
        }

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="type"><see cref="Type"/> of object to get from the container.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Resolve(this IUnityContainer container, Type type, params ResolverOverride[] overrides)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).Resolve(type, null, overrides);
        }

        #endregion


        #region ResolveAll overloads

        /// <summary>
        /// Return instances of all registered types requested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful if you've registered multiple types with the same
        /// <see cref="Type"/> but different names.
        /// </para>
        /// <para>
        /// Be aware that this method does NOT return an instance for the default (unnamed) registration.
        /// </para>
        /// </remarks>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="type">The type requested.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve calls.</param>
        /// <returns>Set of objects of type <paramref name="type"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<object> ResolveAll(this IUnityContainer container, Type type, params ResolverOverride[] resolverOverrides)
        {
            // TODO: ????
            var result = (container ?? throw new ArgumentNullException(nameof(container))).Resolve((type ?? throw new ArgumentNullException(nameof(type))).MakeArrayType(), resolverOverrides);
            return result is IEnumerable<object> objects ? objects : ((Array)result!).Cast<object>();
        }

        /// <summary>
        /// Return instances of all registered types requested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful if you've registered multiple types with the same
        /// <see cref="Type"/> but different names.
        /// </para>
        /// <para>
        /// Be aware that this method does NOT return an instance for the default (unnamed) registration.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type requested.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve calls.</param>
        /// <returns>Set of objects of type <typeparamref name="T"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ResolveAll<T>(this IUnityContainer container, params ResolverOverride[] resolverOverrides)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .ResolveAll(typeof(T), resolverOverrides)
                .Cast<T>();
        }

        #endregion


        #region BuildUp overloads

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// <para>
        /// This overload uses the default registrations.
        /// </para>
        /// </remarks>
        /// <typeparam name="T"><see cref="Type"/> of object to perform injection on.</typeparam>
        /// <param name="container">Container to resolve through.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="resolverOverrides">Any overrides for the buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <typeparamref name="T"/>).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T BuildUp<T>(this IUnityContainer container, T existing, params ResolverOverride[] resolverOverrides)
        {
            if (existing is null) throw new ArgumentNullException(nameof(existing));
            return (T)(container ?? throw new ArgumentNullException(nameof(container))).BuildUp(typeof(T), existing, null, resolverOverrides);
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para></remarks>
        /// <typeparam name="T"><see cref="Type"/> of object to perform injection on.</typeparam>
        /// <param name="container">Container to resolve through.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="name">name to use when looking up the typemappings and other configurations.</param>
        /// <param name="resolverOverrides">Any overrides for the Buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <typeparamref name="T"/>).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T BuildUp<T>(this IUnityContainer container, T existing, string name, params ResolverOverride[] resolverOverrides)
        {
            if (existing is null) throw new ArgumentNullException(nameof(existing));
            if (container is null) throw new ArgumentNullException(nameof(container));
            return (T)container.BuildUp(typeof(T), existing, name, resolverOverrides);
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// <para>
        /// This overload uses the default registrations.
        /// </para>
        /// </remarks>
        /// <param name="container">Container to resolve through.</param>
        /// <param name="type"><see cref="Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="resolverOverrides">Any overrides for the Buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="type"/>).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object BuildUp(this IUnityContainer container, Type type, object existing, params ResolverOverride[] resolverOverrides)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).BuildUp(type, existing, null, resolverOverrides);
        }

        #endregion
    }
}
