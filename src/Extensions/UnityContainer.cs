using System;
using Unity.Extension;

namespace Unity
{
    // Extension Management
    public static class UnityContainerExtensions
    {
        #region Extension management and configuration

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
            if (null == container) throw new ArgumentNullException(nameof(container));

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
            if (null == container) throw new ArgumentNullException(nameof(container));
            // TODO: implement resolution 
            return container.AddExtension(new TExtension());
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="container">Container to add the extension to.</param>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        public static void Add(this UnityContainer container, UnityContainerExtension extension)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));

            container.AddExtension(extension);
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="container">Container to add the extension to.</param>
        /// <param name="type"><see cref="Type"/> of the extension to add</param>
        public static void Add(this UnityContainer container, Type type)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!typeof(UnityContainerExtension).IsAssignableFrom(type)) 
                throw new ArgumentException($"Type {type} must be subclass of 'UnityContainerExtension'", nameof(type));

            var extension = (UnityContainerExtension)Activator.CreateInstance(type);
            container.AddExtension(extension);
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
        public static TConfigurator Configure<TConfigurator>(this UnityContainer container)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));
            
            return (TConfigurator)container.Configure(typeof(TConfigurator));
        }

        #endregion
    }
}
