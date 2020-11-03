using System;
using Unity.Extension;

namespace Unity
{
    // Extension Management
    public static partial class UnityContainerExtensions
    {
        #region UnityContainer

        #region AddExtension

        /// <summary>
        /// Creates a new <typeparamref name="TExtension"/> extension and adds it to the container.
        /// </summary>
        /// <remarks>
        /// This overload requires <typeparamref name="TExtension"/> to implement default constructor.
        /// The extension is creates the by calling 'new TExtension()'. For extensions requiring invoking
        /// constructor with resolved dependencies call <see cref="AddExtension{TExtension}(UnityContainer container)"/> 
        /// method.
        /// </remarks>
        /// <typeparam name="TExtension">Type of <see cref="UnityContainerExtension"/> to add. The extension type
        /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static UnityContainer AddNewExtension<TExtension>(this UnityContainer container)
            where TExtension : UnityContainerExtension, new()
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            return container.AddExtension(new TExtension());
        }

        /// <summary>
        /// Resolves an extension object and adds it to the container.
        /// </summary>
        /// <typeparam name="TExtension">Type of <see cref="UnityContainerExtension"/> to add. The extension type
        /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static UnityContainer AddExtension<TExtension>(this UnityContainer container)
            where TExtension : UnityContainerExtension, new()
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            // TODO: implement resolution 
            return container.AddExtension(new TExtension());
        }

        #endregion


        #region Add

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="container">Container to add the extension to.</param>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        public static void Add(this UnityContainer container, UnityContainerExtension extension)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            container.AddExtension(extension);
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="container">Container to add the extension to.</param>
        /// <param name="type"><see cref="Type"/> of the extension to add</param>
        public static void Add(this UnityContainer container, Type type)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!typeof(UnityContainerExtension).IsAssignableFrom(type)) 
                throw new ArgumentException($"Type {type} must be subclass of 'UnityContainerExtension'", nameof(type));

            var extension = (UnityContainerExtension)Activator.CreateInstance(type)!;
            container.AddExtension(extension);
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="container">Container to add the extension to.</param>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        public static void Add(this UnityContainer container, Action<ExtensionContext> action)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            if (null == action)    throw new ArgumentNullException(nameof(action));

            container.AddExtension(action);
        }

        #endregion


        #region Configuration

        /// <summary>
        /// Get extension of <typeparamref name="TConfigurator"/> type.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <typeparam name="TConfigurator">The configuration interface required.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public static TConfigurator? Configure<TConfigurator>(this UnityContainer container)
            where TConfigurator : class, IUnityContainerExtensionConfigurator
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            
            return (TConfigurator?)container.Configure(typeof(TConfigurator));
        }

        #endregion

        #endregion


        #region IUnityContainer

        #region AddExtension

        /// <summary>
        /// Creates a new <typeparamref name="TExtension"/> extension and adds it to the container.
        /// </summary>
        /// <remarks>
        /// This overload requires <typeparamref name="TExtension"/> to implement default constructor.
        /// The extension is creates the by calling 'new TExtension()'. For extensions requiring invoking
        /// constructor with resolved dependencies call <see cref="AddExtension{TExtension}(UnityContainer container)"/> 
        /// method.
        /// </remarks>
        /// <typeparam name="TExtension">Type of <see cref="UnityContainerExtension"/> to add. The extension type
        /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static IUnityContainer AddNewExtension<TExtension>(this IUnityContainer container)
            where TExtension : UnityContainerExtension, new()
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));

            return unity.AddExtension(new TExtension());
        }

        /// <summary>
        /// Resolves an extension object and adds it to the container.
        /// </summary>
        /// <typeparam name="TExtension">Type of <see cref="UnityContainerExtension"/> to add. The extension type
        /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static IUnityContainer AddExtension<TExtension>(this IUnityContainer container)
            where TExtension : UnityContainerExtension, new()
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));
            // TODO: implement resolution 
            return unity.AddExtension(new TExtension());
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public static IUnityContainer AddExtension(this IUnityContainer container, UnityContainerExtension extension)
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));
            // TODO: implement resolution 
            return unity.AddExtension(extension);
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="method"><see cref="Action{ExtensionContext}"/> delegate</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public static IUnityContainer AddExtension(this IUnityContainer container, Action<ExtensionContext> method)
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));
            
            return unity.AddExtension(method);
        }

        #endregion


        #region Configuration

        /// <summary>
        /// Resolve access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public static object? Configure(this IUnityContainer container, Type configurationInterface)
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));

            return unity.Configure(configurationInterface);
        }

        /// <summary>
        /// Get extension of <typeparamref name="TConfigurator"/> type.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <typeparam name="TConfigurator">The configuration interface required.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public static TConfigurator? Configure<TConfigurator>(this IUnityContainer container)
            where TConfigurator : class, IUnityContainerExtensionConfigurator
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));

            return (TConfigurator?)unity.Configure(typeof(TConfigurator));
        }

        #endregion

        #endregion


        #region IServiceProvider

        #region AddExtension

        /// <summary>
        /// Creates a new <typeparamref name="TExtension"/> extension and adds it to the container.
        /// </summary>
        /// <remarks>
        /// This overload requires <typeparamref name="TExtension"/> to implement default constructor.
        /// The extension is creates the by calling 'new TExtension()'. For extensions requiring invoking
        /// constructor with resolved dependencies call <see cref="AddExtension{TExtension}(UnityContainer container)"/> 
        /// method.
        /// </remarks>
        /// <typeparam name="TExtension">Type of <see cref="UnityContainerExtension"/> to add. The extension type
        /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static UnityContainer AddNewExtension<TExtension>(this IServiceProvider container)
            where TExtension : UnityContainerExtension, new()
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));

            return unity.AddExtension(new TExtension());
        }

        /// <summary>
        /// Resolves an extension object and adds it to the container.
        /// </summary>
        /// <typeparam name="TExtension">Type of <see cref="UnityContainerExtension"/> to add. The extension type
        /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static UnityContainer AddExtension<TExtension>(this IServiceProvider container)
            where TExtension : UnityContainerExtension, new()
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));
            // TODO: implement resolution 
            return unity.AddExtension(new TExtension());
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public static IUnityContainer AddExtension(this IServiceProvider container, UnityContainerExtension extension)
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));
            // TODO: implement resolution 
            return unity.AddExtension(extension);
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="method"><see cref="Action{ExtensionContext}"/> delegate</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public static IUnityContainer AddExtension(this IServiceProvider container, Action<ExtensionContext> method)
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));

            return unity.AddExtension(method);
        }

        #endregion


        #region Configuration


        /// <summary>
        /// Resolve access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public static object? Configure(this IServiceProvider container, Type configurationInterface)
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));

            return unity.Configure(configurationInterface);
        }

        /// <summary>
        /// Get extension of <typeparamref name="TConfigurator"/> type.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <typeparam name="TConfigurator">The configuration interface required.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public static TConfigurator? Configure<TConfigurator>(this IServiceProvider container)
            where TConfigurator : class, IUnityContainerExtensionConfigurator
        {
            if (!(container is UnityContainer unity)) throw new ArgumentNullException(nameof(container));

            return (TConfigurator?)unity.Configure(typeof(TConfigurator));
        }

        #endregion

        #endregion
    }
}
