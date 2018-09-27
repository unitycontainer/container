

using System;
using Unity.Extension;

namespace Unity.Events
{
    /// <summary>
    /// Event argument class for the <see cref="ExtensionContext.ChildContainerCreated"/> event.
    /// </summary>
    public class ChildContainerCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a new <see cref="ChildContainerCreatedEventArgs"/> object with the
        /// given child container object.
        /// </summary>
        /// <param name="childContext">An <see cref="ExtensionContext"/> for the newly created child
        /// container.</param>
        public ChildContainerCreatedEventArgs(ExtensionContext childContext)
        {
            ChildContext = childContext;
        }

        /// <summary>
        /// The newly created child container.
        /// </summary>
        public IUnityContainer ChildContainer => ChildContext.Container;

        /// <summary>
        /// An extension context for the created child container.
        /// </summary>
        public ExtensionContext ChildContext { get; }
    }
}
