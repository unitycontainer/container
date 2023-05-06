using System.Collections;
using Unity.Extension;

namespace Unity
{
    // Extension Management
    public sealed partial class UnityContainer : IEnumerable<Type>
    {
        /// <summary>
        /// This method returns <see cref="IEnumerator{Type}"/> with types of available 
        /// managed extensions
        /// </summary>
        /// <remarks>
        /// Extensions, after executing method <see cref="UnityContainerExtension.Initialize"/>,
        /// either discarded or added to the container's storage based on implementation of 
        /// <see cref="IUnityContainerExtensionConfigurator"/> interface:
        /// <para>If extension implements <see cref="IUnityContainerExtensionConfigurator"/> interface,
        /// it is stored in the container and kept alive until the container goes out of scope.</para>
        /// <para>If extension does not implement the interface, its reference is released 
        /// immediately after initialization</para>
        /// </remarks>
        /// <returns><see cref="Type"/> enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Type>)this).GetEnumerator();

        /// <summary>
        /// This method returns <see cref="IEnumerator{Type}"/> with types of available 
        /// managed extensions
        /// </summary>
        /// <remarks>
        /// Extensions, after executing method <see cref="UnityContainerExtension.Initialize"/>,
        /// either discarded or added to the container's storage based on implementation of 
        /// <see cref="IUnityContainerExtensionConfigurator"/> interface:
        /// <para>If extension implements <see cref="IUnityContainerExtensionConfigurator"/> interface,
        /// it is stored in the container and kept alive until the container goes out of scope.</para>
        /// <para>If extension does not implement the interface, its reference is released 
        /// immediately after initialization</para>
        /// </remarks>
        /// <returns><see cref="Type"/> enumerator</returns>
        IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
        {
            if (_extensions is null) yield break;

            foreach (var extension in _extensions)
                yield return extension.GetType();
        }
    }
}
