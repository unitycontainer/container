﻿using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    public class PerResolveLifetimeManager : LifetimeManager, 
                                             IInstanceLifetimeManager, 
                                             IFactoryLifetimeManager,
                                             ITypeLifetimeManager
    {
        #region Fields

        protected object? value = NoValue;

        #endregion


        #region Constructors

        public PerResolveLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }
        
        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? GetValue(ICollection<IDisposable> lefetime)
        {
            return value;
        }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerResolveLifetimeManager();
        }

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerResolve";

        #endregion
    }
}
