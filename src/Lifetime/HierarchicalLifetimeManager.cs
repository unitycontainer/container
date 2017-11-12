// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Policy;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    public class HierarchicalLifetimeManager : SynchronizedLifetimeManager,
                                               IHierarchicalLifetimePolicy,
                                               ISingletonLifetimePolicy,
                                               IDisposable
    {
        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            Dispose();
        }

        public virtual IBuilderPolicy CreateScope()
        {
            return new HierarchicalLifetimeManager();
        }


        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Standard Dispose pattern implementation.
        /// </summary>
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Value == null) return;
            if (Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
            Value = null;
        }

        #endregion
    }
}
