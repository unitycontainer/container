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
                ? _scope.Contains(type)
                : _scope.Contains(type, name);
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
                            enumerator = _scope.Registrations;

                            _cache = new WeakReference(enumerator);
                            return enumerator;
                        }
                    }
                }

                // Cache registrations
                enumerator = (IEnumerable<ContainerRegistration>?)_cache.Target;
                if (null == enumerator || !_cache.IsAlive)
                {
                    enumerator = _scope.Registrations;
                    _cache.Target = enumerator;
                }
                else
                {
                    var en = enumerator.GetHashCode();
                    var sc = _scope.GetHashCode();
                    if (en != sc)
                    {
                        enumerator = _scope.Registrations;
                        _cache.Target = enumerator;
                    }
                }

                return enumerator!;
            }
        }

        #endregion
    }
}
