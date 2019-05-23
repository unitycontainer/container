using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the
    /// <see cref="IUnityContainerAsync"/> interface.
    /// </summary>
    public static class UnityContainerAsyncExtensions
    {
        #region RegisterType overloads

        #region Generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <typeparam name="T">Type this registration is for.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType<T>(this IUnityContainerAsync container, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, typeof(T), null, null, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType<T>(this IUnityContainerAsync container, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, typeof(T), null, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <typeparam name="T">The type to configure injection on.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name that will be used to request the type.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType<T>(this IUnityContainerAsync container, string name, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, typeof(T), name, null, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType<T>(this IUnityContainerAsync container, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, typeof(T), name, lifetimeManager, injectionMembers);
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
        public static Task RegisterType<TFrom, TTo>(this IUnityContainerAsync container, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(new[] { typeof(TFrom) }, typeof(TTo), null, null, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType<TFrom, TTo>(this IUnityContainerAsync container, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(new[] { typeof(TFrom) }, typeof(TTo), null, lifetimeManager, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType<TFrom, TTo>(this IUnityContainerAsync container, string name, params InjectionMember[] injectionMembers)
            where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(new[] { typeof(TFrom) }, typeof(TTo), name, null, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType<TFrom, TTo>(this IUnityContainerAsync container, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(new[] { typeof(TFrom) }, typeof(TTo), name, lifetimeManager, injectionMembers);
        }


        /// <summary>
        /// Register a type with specific members to be injected as singleton.
        /// </summary>
        /// <typeparam name="T">Type this registration is for.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterSingleton<T>(this IUnityContainerAsync container, params InjectionMember[] injectionMembers)
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
        public static Task RegisterSingleton<T>(this IUnityContainerAsync container, string name, params InjectionMember[] injectionMembers)
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
        public static Task RegisterSingleton<TFrom, TTo>(this IUnityContainerAsync container, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(new[] { typeof(TFrom) }, typeof(TTo), null, new ContainerControlledLifetimeManager(), injectionMembers);
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
        public static Task RegisterSingleton<TFrom, TTo>(this IUnityContainerAsync container, string name, params InjectionMember[] injectionMembers)
            where TTo : TFrom
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(new[] { typeof(TFrom) }, typeof(TTo), name, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        #endregion

        #region Non-generics overloads

        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">Type this registration is for.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType(this IUnityContainerAsync container, Type t, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, t, null, null, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">The <see cref="Type"/> to apply the <paramref name="lifetimeManager"/> to.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType(this IUnityContainerAsync container, Type t, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, t, null, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">The <see cref="Type"/> to configure in the container.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType(this IUnityContainerAsync container, Type t, string name, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, t, name, null, injectionMembers);
        }

        /// <summary>
        /// Register a <see cref="LifetimeManager"/> for the given type and name with the container.
        /// No type mapping is performed for this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">The <see cref="Type"/> to apply the <paramref name="lifetimeManager"/> to.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType(this IUnityContainerAsync container, Type t, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(null, t, name, lifetimeManager, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType(this IUnityContainerAsync container, IEnumerable<Type> from, Type to, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(from, to, null, null, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType(this IUnityContainerAsync container, IEnumerable<Type> from, Type to, string name, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(from, to, name, null, injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterType(this IUnityContainerAsync container, IEnumerable<Type> from, Type to, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(from, to, null, lifetimeManager, injectionMembers);
        }


        /// <summary>
        /// Register a type with specific members to be injected.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="t">Type this registration is for.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterSingleton(this IUnityContainerAsync container, Type t, params InjectionMember[] injectionMembers)
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
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterSingleton(this IUnityContainerAsync container, Type t, string name, params InjectionMember[] injectionMembers)
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
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterSingleton(this IUnityContainerAsync container, IEnumerable<Type> from, Type to, params InjectionMember[] injectionMembers)
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
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterSingleton(this IUnityContainerAsync container, IEnumerable<Type> from, Type to, string name, params InjectionMember[] injectionMembers)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterType(from, to, name, new ContainerControlledLifetimeManager(), injectionMembers);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterInstance<TInterface>(this IUnityContainerAsync container, TInterface instance)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(new[] { typeof(TInterface) }, null, instance, new ContainerControlledLifetimeManager());
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterInstance<TInterface>(this IUnityContainerAsync container, TInterface instance, IInstanceLifetimeManager lifetimeManager)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(new[] { typeof(TInterface) }, null, instance, lifetimeManager);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterInstance<TInterface>(this IUnityContainerAsync container, string name, TInterface instance)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(new[] { typeof(TInterface) }, name, instance, new ContainerControlledLifetimeManager());
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterInstance<TInterface>(this IUnityContainerAsync container, string name, TInterface instance, IInstanceLifetimeManager lifetimeManager)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).RegisterInstance(new[] { typeof(TInterface) }, name, instance, lifetimeManager);
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
        /// <param name="t">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterInstance(this IUnityContainerAsync container, Type t, object instance)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(new[] { t }, null, instance, new ContainerControlledLifetimeManager());
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
        /// <param name="t">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> object that controls how this instance will be managed by the container.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterInstance(this IUnityContainerAsync container, Type t, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(new[] { t }, null, instance, lifetimeManager);
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
        /// <param name="t">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="name">Name for registration.</param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterInstance(this IUnityContainerAsync container, Type t, string name, object instance)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterInstance(new[] { t }, name, instance, new ContainerControlledLifetimeManager());
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
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. If no manager is provided, container uses Transient manager.</param>
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterFactory<TInterface>(this IUnityContainerAsync container, Func<IUnityContainer, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(new[] { typeof(TInterface) }, null, (c, t, n) => factory(c), lifetimeManager);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterFactory<TInterface>(this IUnityContainerAsync container, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(new[] { typeof(TInterface) }, null, factory, lifetimeManager);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterFactory<TInterface>(this IUnityContainerAsync container, string name, Func<IUnityContainer, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(new[] { typeof(TInterface) }, name, (c, t, n) => factory(c), lifetimeManager);
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
        /// <returns>The <see cref="Unity.IUnityContainer"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task RegisterFactory<TInterface>(this IUnityContainerAsync container, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(new[] { typeof(TInterface) }, name, factory, lifetimeManager);
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
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainerAsync, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainerAsync RegisterFactory(this IUnityContainerAsync container, Type type, Func<IUnityContainerAsync, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(type, null, (c, t, n) => factory(c), lifetimeManager);
        }

        /// <summary>
        /// Register a Factory with the container.
        /// </summary>
        /// <remarks>
        /// This overload does a default registration and has the current container take over the lifetime of the factory.
        /// </remarks>
        /// <param name="container">Container to configure.</param>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainerAsync, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainerAsync RegisterFactory(this IUnityContainerAsync container, Type type, Func<IUnityContainerAsync, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(type, null, factory, lifetimeManager);
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
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainerAsync, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainerAsync RegisterFactory(this IUnityContainerAsync container, Type type, string name, Func<IUnityContainerAsync, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(type, name, (c, t, n) => factory(c), lifetimeManager);
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
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainerAsync, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="Unity.IUnityContainerAsync"/> object that this method was called on .</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUnityContainerAsync RegisterFactory(this IUnityContainerAsync container, Type type, string name, Func<IUnityContainerAsync, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager = null)
        {
            return (container ?? throw new ArgumentNullException(nameof(container)))
                .RegisterFactory(type, name, factory, lifetimeManager);
        }

        #endregion

        #endregion


        #region Resolve overloads


        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of object to get from the container.</typeparam>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecuritySafeCritical]
        public static T ResolveAsync<T>(this IUnityContainerAsync container, params ResolverOverride[] overrides)
        {
            var unity = container ?? throw new ArgumentNullException(nameof(container));
            var task  = unity.ResolveAsync(typeof(T), null, overrides);

            return task.IsCompleted
                ? (T)task.Result
                : (T)task.GetAwaiter().GetResult();
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
        [SecuritySafeCritical]
        public static T ResolveAsync<T>(this IUnityContainerAsync container, string name, params ResolverOverride[] overrides)
        {
            var unity = container ?? throw new ArgumentNullException(nameof(container));
            var task = unity.ResolveAsync(typeof(T), name, overrides);

            return task.IsCompleted
                ? (T)task.Result
                : (T)task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Resolve an instance of the default requested type from the container.
        /// </summary>
        /// <param name="container">Container to resolve from.</param>
        /// <param name="t"><see cref="Type"/> of object to get from the container.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecuritySafeCritical]
        public static ValueTask<object> ResolveAsync(this IUnityContainerAsync container, Type t, params ResolverOverride[] overrides)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).ResolveAsync(t, null, overrides);
        }

        #endregion


        #region Registration Helpers

        /// <summary>
        /// Check if a particular type has been registered with the container with
        /// the default name.
        /// </summary>
        /// <param name="container">Container to inspect.</param>
        /// <param name="typeToCheck">Type to check registration for.</param>
        /// <returns>True if this type has been registered, false if not.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered(this IUnityContainerAsync container, Type typeToCheck)
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
        public static bool IsRegistered<T>(this IUnityContainerAsync container)
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
        public static bool IsRegistered<T>(this IUnityContainerAsync container, string nameToCheck)
        {
            return (container ?? throw new ArgumentNullException(nameof(container))).IsRegistered(typeof(T), nameToCheck);
        }

        #endregion
    }
}
