using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the
    /// <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static partial class UnityNamespaceExtensions
    {
        #region Generics overloads

        /// <summary>
        /// Register a type with specific members to be injected as singleton.
        /// </summary>
        /// <typeparam name="T">Type this registration is for.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton<T>(this IUnityContainer container, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, typeof(T), null, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Register type as a singleton.
        /// </summary>
        /// <typeparam name="T">The type to configure injection on.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name that will be used to request the type.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton<T>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, typeof(T), name, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used to tell the container that when asked for type <typeparamref name="TFrom"/>,
        /// actually return an instance of type <typeparamref name="TTo"/>. This is very useful for
        /// getting instances of interfaces.
        /// </para>
        /// <para>
        /// This overload registers a default mapping and transient lifetime.
        /// </para>
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton<TFrom, TTo>(this IUnityContainer container, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(typeof(TFrom), typeof(TTo), null, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Register a type mapping as singleton.
        /// </summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for type <typeparamref name="TFrom"/>,
        /// actually return an instance of type <typeparamref name="TTo"/>. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton<TFrom, TTo>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers)
            where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(typeof(TFrom), typeof(TTo), name, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        #endregion


        #region Non-generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">Type this registration is for.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton(this IUnityContainer container, Type t, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, t, null, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">The <see cref="Type"/> to configure in the container.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton(this IUnityContainer container, Type t, string name, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, t, name, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used to tell the container that when asked for type <paramref name="from"/>,
        /// actually return an instance of type <paramref name="to"/>. This is very useful for
        /// getting instances of interfaces.
        /// </para>
        /// <para>
        /// This overload registers a default mapping.
        /// </para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton(this IUnityContainer container, Type from, Type to, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(from, to, null, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for type <paramref name="from"/>,
        /// actually return an instance of type <paramref name="to"/>. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterSingleton(this IUnityContainer container, Type from, Type to, string name, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(from, to, name, new ContainerControlledLifetimeManager(), injectionMembers);
        }
        #endregion
    }
}
