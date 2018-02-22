using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Events;
using Unity.Extension;
using Unity.Policy;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Public Constructor

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
            : this(null)
        {
        }

        #endregion


        #region Extension Management

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer AddExtension(UnityContainerExtension extension)
        {
            _extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
            extension.InitializeExtension(_context);

            return this;
        }

        /// <summary>
        /// GetOrDefault access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public object Configure(Type configurationInterface)
        {
            return _extensions.FirstOrDefault(ex => configurationInterface.GetTypeInfo()
                              .IsAssignableFrom(ex.GetType().GetTypeInfo()));
        }

        /// <summary>
        /// Remove all installed extensions typeFrom this container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method removes all extensions typeFrom the container, including the default ones
        /// that implement the out-of-the-box behavior. After this method, if you want to use
        /// the container again you will need to either read the default extensions or replace
        /// them with your own.
        /// </para>
        /// <para>
        /// The registered instances and singletons that have already been set up in this container
        /// do not get removed.
        /// </para>
        /// </remarks>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RemoveAllExtensions()
        {
            var toRemove = new List<UnityContainerExtension>(_extensions);
            toRemove.Reverse();
            foreach (UnityContainerExtension extension in toRemove)
            {
                extension.Remove();
                (extension as IDisposable)?.Dispose();
            }

            _extensions.Clear();

            // Reset our policies, strategies, and registered names to reset to "zero"
            _strategies = new StagedStrategyChain<IBuilderStrategy, UnityBuildStage>(_parent?._strategies);

            _context.ClearAll();

            if (null == _parent)
                InitializeRootContainer();

            return this;
        }

        #endregion


        #region Child container management

        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different
        /// settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        public IUnityContainer CreateChildContainer()
        {
            var child = new UnityContainer(this);
            ChildContainerCreated?.Invoke(this, new ChildContainerCreatedEventArgs(child._context));
            return child;
        }

        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <value>The parent container, or null if this container doesn'type have one.</value>
        public IUnityContainer Parent => _parent;

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// Disposing the container also disposes any child containers,
        /// and disposes any instances whose lifetimes are managed
        /// by the container.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// This class doesn'type have a finalizer, so <paramref name="disposing"/> will always be true.</remarks>
        /// <param name="disposing">True if being called typeFrom the IDisposable.Dispose
        /// method, false if being called typeFrom a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var exceptions = new List<Exception>();

                try
                {
                    _parent?._lifetimeContainer?.Remove(this);
                    _lifetimeContainer?.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

                foreach (IDisposable disposable in _extensions.OfType<IDisposable>().ToArray())
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }

                _extensions.Clear();
                _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(1);

                if (exceptions.Count == 1)
                {
                    throw exceptions.First();
                }
                else if (exceptions.Count > 1)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        #endregion
    }
}
