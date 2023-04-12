using System;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;

// TODO: Requires verification
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
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <typeparam name="T">Type this registration is for.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, typeof(T), null, null, injectionMembers);

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to apply the <paramref name="lifetimeManager"/> to.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, typeof(T), null, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to configure injection on.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name that will be used to request the type.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string contractName, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, typeof(T), contractName, null, injectionMembers);

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to apply the <paramref name="lifetimeManager"/> to.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name that will be used to request the type.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string contractName, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, typeof(T), contractName, lifetimeManager, injectionMembers);

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
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, params InjectionMember[] injectionMembers)
            where TTo : TFrom => container.RegisterType(typeof(TFrom), typeof(TTo), null, null, injectionMembers);

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            where TTo : TFrom => container.RegisterType(typeof(TFrom), typeof(TTo), null, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for type <typeparamref name="TFrom"/>,
        /// actually return an instance of type <typeparamref name="TTo"/>. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name of this mapping.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string contractName, params InjectionMember[] injectionMembers)
            where TTo : TFrom => container.RegisterType(typeof(TFrom), typeof(TTo), contractName, null, injectionMembers);

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string contractName, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            where TTo : TFrom => container.RegisterType(typeof(TFrom), typeof(TTo), contractName, lifetimeManager, injectionMembers);

        #endregion


        #region Non-generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType">Type this registration is for.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type contractType, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, contractType, null, null, injectionMembers);

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="type">The <see cref="Type"/> to apply the <paramref name="lifetimeManager"/> to.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type type, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, type, null, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType">The <see cref="Type"/> to configure in the container.</param>
        /// <param name="contractName">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type contractType, string contractName, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, contractType, contractName, null, injectionMembers);

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType">The <see cref="Type"/> to apply the <paramref name="lifetimeManager"/> to.</param>
        /// <param name="contractName">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type contractType, string contractName, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterType(null, contractType, contractName, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used to tell the container that when asked for type <paramref name="contractType"/>,
        /// actually return an instance of type <paramref name="implementationType"/>. This is very useful for
        /// getting instances of interfaces.
        /// </para>
        /// <para>
        /// This overload registers a default mapping.
        /// </para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type contractType, Type implementationType, params InjectionMember[] injectionMembers) 
            => container.RegisterType(contractType, implementationType, null, null, injectionMembers);

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type contractType, Type implementationType, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterType(contractType, implementationType, null, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register a type mapping with the container.
        /// </summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for type <paramref name="contractType"/>,
        /// actually return an instance of type <paramref name="implementationType"/>. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="contractName">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type contractType, Type implementationType, string contractName, params InjectionMember[] injectionMembers) 
            => container.RegisterType(contractType, implementationType, contractName, null, injectionMembers);


        #endregion
    }
}
