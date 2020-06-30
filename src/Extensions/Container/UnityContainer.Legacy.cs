using System;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the
    /// <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static partial class UnityContainerExtensions
    {
        #region RegisterType overloads

        #region Generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <typeparam name="T">Type this registration is for.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(T), null, new TransientLifetimeManager(injectionMembers));
        }

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
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, ITypeLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager) 
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(T), null, lifetimeManager);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to configure injection on.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name that will be used to request the type.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string name, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(T), name, new TransientLifetimeManager(injectionMembers));
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to apply the <paramref name="lifetimeManager"/> to.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name that will be used to request the type.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<T>(this IUnityContainer container, string name, ITypeLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(T), name, lifetimeManager);
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
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, 
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(TTo), null, new TransientLifetimeManager(injectionMembers), typeof(TFrom));
        }

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
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, ITypeLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(TTo), null, lifetimeManager, typeof(TFrom));
        }

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
        /// <param name="name">Name of this mapping.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string name, 
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(TTo), name, new TransientLifetimeManager(injectionMembers), typeof(TFrom));
        }

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <typeparam name="TFrom"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TTo"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType<TFrom, TTo>(this IUnityContainer container, string name, ITypeLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(typeof(TTo), name, lifetimeManager, typeof(TFrom));
        }

        #endregion

        #region Non-generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="type">Type this registration is for.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type type, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(type, null, new TransientLifetimeManager(injectionMembers));
        }

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
        public static IUnityContainer RegisterType(this IUnityContainer container, Type type, ITypeLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(type, null, lifetimeManager);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="type">The <see cref="Type"/> to configure in the container.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type type, string name, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(type, name, new TransientLifetimeManager(injectionMembers));
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="type">The <see cref="Type"/> to apply the <paramref name="lifetimeManager"/> to.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type type, string name, ITypeLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(type, name, lifetimeManager);
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
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(to, null, new TransientLifetimeManager(injectionMembers), from);
        }

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, ITypeLifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(to, null, lifetimeManager, from);
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
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, string name, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(to, name, new TransientLifetimeManager(injectionMembers), from);
        }

        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="from"><see cref="Type"/> that will be requested.</param>
        /// <param name="to"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterType(this IUnityContainer container, Type from, Type to, string name, ITypeLifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterType(to, name, lifetimeManager, from);
        }


        #endregion

        #endregion


        #region RegisterInstance overloads

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
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, null, new ContainerControlledLifetimeManager(injectionMembers), typeof(TInterface));
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
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, TInterface instance, IInstanceLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, null, lifetimeManager, typeof(TInterface));
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
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, TInterface instance, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, name, new ContainerControlledLifetimeManager(injectionMembers), typeof(TInterface));
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
        public static IUnityContainer RegisterInstance<TInterface>(this IUnityContainer container, string name, TInterface instance, IInstanceLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, name, lifetimeManager, typeof(TInterface));
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
        /// This overload does a default registration and has the container take over the lifetime of the instance.</para>
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, object instance, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, null, new ContainerControlledLifetimeManager(injectionMembers), type);
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
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, object instance, IInstanceLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, null, lifetimeManager, type);
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
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, string name, object instance, 
            params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, name, new ContainerControlledLifetimeManager(injectionMembers), type);
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
        public static IUnityContainer RegisterInstance(this IUnityContainer container, Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(instance, name, lifetimeManager, type);
        }

        #endregion

        #endregion


        #region RegisterFactory overloads

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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, object?> factory, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, new TransientLifetimeManager(injectionMembers), typeof(TInterface));
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, lifetimeManager, typeof(TInterface));
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, object?> factory,
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, new TransientLifetimeManager(injectionMembers), typeof(TInterface));
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, lifetimeManager, typeof(TInterface));
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, Type, string?, object?> factory, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, new TransientLifetimeManager(injectionMembers), typeof(TInterface));
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, lifetimeManager, typeof(TInterface));
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, Type, string?, object?> factory, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, new TransientLifetimeManager(injectionMembers), typeof(TInterface));
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
        public static IUnityContainer RegisterFactory<TInterface>(this IUnityContainer container, string name, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, lifetimeManager, typeof(TInterface));
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, object> factory, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, new TransientLifetimeManager(injectionMembers), type);
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, object?> factory, IFactoryLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, lifetimeManager, type);
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, object?> factory,
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, new TransientLifetimeManager(injectionMembers), type);
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, object> factory, IFactoryLifetimeManager lifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, lifetimeManager, type);
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, Type, string?, object?> factory, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, new TransientLifetimeManager(injectionMembers), type);
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, null, lifetimeManager, type);
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, Type, string?, object?> factory, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, new TransientLifetimeManager(injectionMembers), type);
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
        public static IUnityContainer RegisterFactory(this IUnityContainer container, Type type, string name, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager lifetimeManager, 
            params InjectionMember[] injectionMembers)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            if (lifetimeManager is LifetimeManager manager)
                manager.InjectionMembers = injectionMembers;
            else
                throw new ArgumentNullException(nameof(lifetimeManager));

            ResolveDelegate<IResolveContext> resolver = (ref IResolveContext context) =>
            {
                var container = (IUnityContainer?)context.Resolve(typeof(IUnityContainer), null);
                return factory(container!, context.Type, context.Name);
            };

            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(resolver, name, lifetimeManager, type);
        }

        #endregion

        #endregion
    }
}
