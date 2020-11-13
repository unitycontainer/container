using System;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

// TODO: Requires verification
namespace Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the
    /// <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static class UnityRegisterFactoryExtensions
    {
        #region Generics overloads

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, object?> factory, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), null, (c, t, n, o) => factory(c), null, injectionMembers);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), null, (c, t, n, o) => factory(c), lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, object?> factory, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), name, (c, t, n, o) => factory(c), null, injectionMembers);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), name, (c, t, n, o) => factory(c), lifetimeManager, injectionMembers);
        }


        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, Type, string?, object?> factory, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), null, (c, t, n, o) => factory(c, t, n), null, injectionMembers);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), null, (c, t, n, o) => factory(c, t, n), lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, Type, string?, object?> factory, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null)   throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), name, (c, t, n, o) => factory(c, t, n), null, injectionMembers);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(typeof(TInterface), name, (c, t, n, o) => factory(c, t, n), lifetimeManager, injectionMembers);
        }

        #endregion


        #region Non-generic overloads

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, object> factory, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .Register(new RegistrationDescriptor((c, t, n, o) => factory(c), null, (IFactoryLifetimeManager)LifetimeManager._factoryManager.Clone(injectionMembers), type));
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(type, null, (c, t, n, o) => factory(c), lifetimeManager, injectionMembers);
        }


        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, object?> factory, params InjectionMember[] injectionMembers)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return container.RegisterFactory(type, name, (c, t, n, o) => factory(c), null, injectionMembers);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, object> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container))).Register(
                new RegistrationDescriptor((c, t, n, o) => factory(c), name, lifetimeManager, type));
        }


        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, Type, string?, object?> factory, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return (container ?? throw new ArgumentNullException(nameof(container))).Register(
                new RegistrationDescriptor((c, t, n, o) => factory(c, t, n), null, (IFactoryLifetimeManager)LifetimeManager._factoryManager.Clone(injectionMembers), type));
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container))).Register(
                new RegistrationDescriptor((c, t, n, o) => factory(c, t, n), null, lifetimeManager, type));
        }


        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, Type, string?, object?> factory, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return (container ?? throw new ArgumentNullException(nameof(container))).Register(
                new RegistrationDescriptor((c, t, n, o) => factory(c, t, n), name, (IFactoryLifetimeManager)LifetimeManager._factoryManager.Clone(injectionMembers), type));
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.Add(injectionMembers);
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container))).Register(
                new RegistrationDescriptor((c, t, n, o) => factory(c, t, n), name, lifetimeManager, type));
        }

        #endregion
    }
}
