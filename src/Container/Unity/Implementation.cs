using System;
using System.Collections.Generic;

namespace Unity
{
    public partial class UnityContainer
    {

        #region Child Containers

        /// <summary>
        /// Creates a child container with given name
        /// </summary>
        /// <param name="name">Name of the child container</param>
        /// <returns>Instance of child <see cref="UnityContainer"/> container</returns>
        private UnityContainer CreateChildContainer(string? name = null)
        {
            // Create child container
            var container = new UnityContainer(this, name);

            // Add to lifetime manager
            ((ICollection<IDisposable>)_scope).Add(container);
            
            // Raise event if required
            _root.ChildContainerCreated?.Invoke(container._context = new PrivateExtensionContext(container));

            return container;
        }

        #endregion
    }
}
