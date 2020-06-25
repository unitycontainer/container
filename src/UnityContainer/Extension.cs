using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Extension;
using Unity.Policy;

namespace Unity
{
    // Extension Management
    public partial class UnityContainer
    {
        #region Fields

        private List<UnityContainerExtension> _extensions = new List<UnityContainerExtension>();
        private ExtensionContext _context;

        #endregion


        #region Extension Context Implementation

        /// <summary>
        /// Implementation of the ExtensionContext that is used extension management.
        /// </summary>
        /// <remarks>
        /// This is a nested class so that it can access state in the container that 
        /// would otherwise be inaccessible.
        /// </remarks>
        [DebuggerTypeProxy(typeof(ExtensionContext))]
        private class ExtensionContextImpl : ExtensionContext
        {
            #region Constructors

            public ExtensionContextImpl(UnityContainer container) => 
                Container = container;

            #endregion


            #region ExtensionContext

            public override IPolicyList Policies => throw new NotImplementedException();

            public override UnityContainer Container { get; }

            #endregion
        }

        #endregion
    }
}
