using System;
using System.Collections.Generic;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

//        private WeakReference? _cache;

        #endregion


        #region Get and Check Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            var scope = _scope;

            if (null == name)
            {
                do
                {
                    if (scope.Contains(type))
                        return true;
                }
                while (null != (scope = scope.Parent));
            }
            else
            {
                do
                {
                    if (_scope.Contains(type, name)) 
                        return true;
                }
                while (null != (scope = scope.Parent));
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                throw new NotImplementedException();

                //IEnumerable<ContainerRegistration>? enumerator;

                //// Initialize collection cache
                //if (null == _cache)
                //{
                //    lock (_scope)
                //    {
                //        if (null == _cache)
                //        {
                //            enumerator = _scope.GetRegistrations;

                //            _cache = new WeakReference(enumerator);
                //            return enumerator;
                //        }
                //    }
                //}

                //// Cache registrations
                //enumerator = (IEnumerable<ContainerRegistration>?)_cache.Target;
                //if (null == enumerator || !_cache.IsAlive)
                //{
                //    enumerator = _scope.GetRegistrations;
                //    _cache.Target = enumerator;
                //}
                //else
                //{
                //    var en = enumerator.GetHashCode();
                //    var sc = _scope.GetHashCode();
                //    if (en != sc)
                //    {
                //        enumerator = _scope.GetRegistrations;
                //        _cache.Target = enumerator;
                //    }
                //}

                //return enumerator!;
            }
        }

        #endregion
    }
}
