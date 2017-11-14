// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Lifetime
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds onto the instance given to it.
    /// When the <see cref="ContainerControlledLifetimeManager"/> is disposed,
    /// the instance is disposed with it.
    /// </summary>
    public class ContainerControlledLifetimeManager : SynchronizedLifetimeManager,
                                                      ISingletonLifetimePolicy,
                                                      IBuildPlanPolicy, 
                                                      IDisposable
    {

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            Dispose();
        }


        public void BuildUp(IBuilderContext context)
        {
            context.Existing = GetValue();
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
