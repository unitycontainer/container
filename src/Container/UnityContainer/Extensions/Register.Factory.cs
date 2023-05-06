using System.Runtime.CompilerServices;
using Unity.Lifetime;

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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, object?> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), null, (c, t, n) => factory(c), null);
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), null, (c, t, n) => factory(c), lifetimeManager);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string contractName, Func<IUnityContainer, object?> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), contractName, (c, t, n) => factory(c), null);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string contractName, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), contractName, (c, t, n) => factory(c), lifetimeManager);
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, Type, string?, object?> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), null, (c, t, n) => factory(c, t, n), null);
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), null, (c, t, n) => factory(c, t, n), lifetimeManager);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string contractName, Func<IUnityContainer, Type, string?, object?> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), contractName, (c, t, n) => factory(c, t, n), null);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <typeparam name="TInterface">Type of instance to register (may be an implemented interface instead of the full type).</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string contractName, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(typeof(TInterface), contractName, (c, t, n) => factory(c, t, n), lifetimeManager);
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
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, Func<IUnityContainer, object> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, null, (c, t, n) => factory(c), null);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, null, (c, t, n) => factory(c), lifetimeManager);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, string contractName, Func<IUnityContainer, object?> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, contractName, (c, t, n) => factory(c), null);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, string contractName, Func<IUnityContainer, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, contractName, (c, t, n) => factory(c), lifetimeManager);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, Func<IUnityContainer, Type, string?, object?> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, null, (c, t, n) => factory(c, t, n), null);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, null, (c, t, n) => factory(c, t, n), lifetimeManager);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, string contractName, Func<IUnityContainer, Type, string?, object?> factory)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, contractName, (c, t, n) => factory(c, t, n), null);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="contractType"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="contractName">Name for registration.</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type contractType, string contractName, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return container.RegisterFactory(contractType, contractName, (c, t, n) => factory(c, t, n), lifetimeManager);
        }

        #endregion
    }
}
