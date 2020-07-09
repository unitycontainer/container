using System;
using System.Collections.Generic;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private WeakReference? _cache;

        #endregion


        #region Get and Check Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            return null == name
                ? _scope.IsRegistered(type)
                : _scope.IsRegistered(type, name);
        }

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                IEnumerable<ContainerRegistration>? enumerator;

                // Initialize collection cache
                if (null == _cache)
                {
                    lock (_scope)
                    {
                        if (null == _cache)
                        {
                            Registering += OnCacheInvalidated;
                            enumerator = _scope.Registrations;

                            _cache = new WeakReference(enumerator);
                            return enumerator;
                        }
                    }
                }

                // Cache registrations
                enumerator = (IEnumerable<ContainerRegistration>?)_cache.Target;
                if (!_cache.IsAlive)
                {
                    enumerator = _scope.Registrations;
                    _cache.Target = enumerator;
                }

                return enumerator!;
            }
        }

        #endregion


        #region Implementation

        private void OnCacheInvalidated(object container, ref RegistrationData registration)
        {
            lock (_scope)
            {
                Registering -= OnCacheInvalidated;
                _cache = null;
            }
        }

        #endregion
    }
}
