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
    public static class UnityRegisterInstanceExtensions
    {
        #region Generics overloads

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration and has the container take over the lifetime of the instance.</para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="instance">Object to returned.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(typeof(TInterface), null, instance, null, injectionMembers);

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration (name = null).
        /// </para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(typeof(TInterface), null, instance, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload automatically has the container take ownership of the <paramref name="instance"/>.</para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="instance">Object to returned.</param>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name for registration.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string? contractName, TInterface instance, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(typeof(TInterface), contractName, instance, null, injectionMembers);

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="instance">Object to returned.</param>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string? contractName, object? instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(typeof(TInterface), contractName, instance, lifetimeManager, injectionMembers);

        #endregion


        #region Non-generic overloads

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload automatically has the container take ownership of the <paramref name="instance"/>.</para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="name">Name for registration.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, object instance, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(null, null, instance, null, injectionMembers);

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration and has the container take over the lifetime of the instance.</para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type contractType, object instance, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(contractType, null, instance, null, injectionMembers);

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration (name = null).
        /// </para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type contractType, object instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(contractType, null, instance, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload automatically has the container take ownership of the <paramref name="instance"/>.</para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="contractName">Name for registration.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type contractType, string? contractName, object instance, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(contractType, contractName, instance, null, injectionMembers);

        public static IUnityContainer RegisterInstance(this IUnityContainer container, string? contractName, object instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(null, contractName, instance, lifetimeManager, injectionMembers);

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// <para>
        /// This overload does a default registration (name = null).
        /// </para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type contractType, string? contractName, object instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
            => container.RegisterInstance(contractType, contractName, instance, lifetimeManager, injectionMembers);

        #endregion
    }
}
