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
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, object? instance, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            return container.Register(new RegistrationDescriptor(instance, null, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(injectionMembers), typeof(TInterface)));
        }

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
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, object? instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return container.Register(new RegistrationDescriptor(instance, null, lifetimeManager, typeof(TInterface)));
        }

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
        /// <param name="name">Name for registration.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, object? instance, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            return container.Register(new RegistrationDescriptor(instance, name, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(injectionMembers), typeof(TInterface)));
        }

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
        /// <param name="name">Name for registration.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, object? instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return container.Register(new RegistrationDescriptor(instance, name, lifetimeManager, typeof(TInterface)));
        }

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
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (instance is null) throw new ArgumentNullException(nameof(instance));

            return container.Register(new RegistrationDescriptor(instance, null, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(injectionMembers), instance.GetType()));
        }

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
        /// <param name="type">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, object instance, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            return container.Register(new RegistrationDescriptor(instance, null, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(injectionMembers), type));
        }

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
        /// <param name="type">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, object instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return container.Register(new RegistrationDescriptor(instance, null, lifetimeManager, type ?? instance.GetType()));
        }

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
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, string name, object instance, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            return container.Register(new RegistrationDescriptor(instance, name, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(injectionMembers), type ?? instance.GetType()));
        }

        public static IUnityContainer RegisterInstance(this IUnityContainer container, string name, object instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (instance is null) throw new ArgumentNullException(nameof(instance));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return container.Register(new RegistrationDescriptor(instance, name, lifetimeManager, instance.GetType()));
        }

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
        public static IUnityContainer RegisterInstance(this IUnityContainer container, string name, object instance, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (instance is null) throw new ArgumentNullException(nameof(instance));

            return container.Register(new RegistrationDescriptor(instance, name, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(injectionMembers), instance.GetType()));
        }

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
        /// <param name="type">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return container.Register(new RegistrationDescriptor(instance, name, lifetimeManager, type));
        }

        #endregion
    }
}
