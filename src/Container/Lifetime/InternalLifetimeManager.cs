﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Unity.Lifetime;

namespace Unity.Container
{
    /// <summary>
    /// Internal container lifetime manager. 
    /// </summary>
    internal class InternalLifetimeManager : LifetimeManager
    {
        #region Fields

        private readonly object? _value;

        #endregion


        #region Constructors

        internal InternalLifetimeManager(object? data, RegistrationCategory type = RegistrationCategory.Instance)
        {
            _value = data;
            Category = type;
        }

        internal InternalLifetimeManager(RegistrationCategory type)
        {
            _value = UnityContainer.NoValue;
            Category = type;
        }

        #endregion

        
        #region Value

        /// <inheritdoc/>
        public override object? TryGetValue(ICollection<IDisposable> _) 
            => _value;

        /// <inheritdoc/>
        public override object? GetValue(ICollection<IDisposable> _) 
            => _value;

        #endregion


        #region Implementation

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
            => throw new NotSupportedException();

        public override ImportSource Source => ImportSource.Local;

        /// <inheritdoc/>
        public override CreationPolicy CreationPolicy => CreationPolicy.Shared;

        /// <inheritdoc/>
        public override string ToString() => "Lifetime: Internal";
        
        #endregion
    }
}