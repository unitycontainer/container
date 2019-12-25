using System;
using Unity.Builder;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Processors;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Scope
{
    /// <summary>
    /// Abstraction layer between container and extensions
    /// </summary>
    /// <remarks>
    /// Implemented as a nested class to gain access to  
    /// container that would otherwise be inaccessible.
    /// </remarks>
    public class ContainerContext 
    {
        #region Fields


        #endregion


        #region Constructors

        public ContainerContext(UnityContainer container)
        {
        }

        #endregion
    }
}
