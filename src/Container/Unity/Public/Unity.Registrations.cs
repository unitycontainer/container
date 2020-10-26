using System;
using System.Collections.Generic;
using Unity.Storage;
using static Unity.Container.Scope;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private Metadata[]? _cache;

        #endregion


        #region Is Registered

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name) => _scope.Contains(new Contract(type, name));


        #endregion


        #region Registrations collection

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                var set = new ScopeSet(_scope);
                var enumerator = new Enumerator(this);

                while (enumerator.MoveNext())
                {
                    //if (set.Contains(in enumerator.Contract))
                    //    continue;

                    var manager = enumerator.Internal.Manager;
                    if (null == manager || RegistrationCategory.Internal > manager.Category)
                        continue;

                    yield return enumerator.Registration;
                }
            }
        }

        public IEnumerable<ContainerRegistration> OtherRegistrations
        {
            get
            {
                if (null == _cache || _scope.Version != _cache.Version())
                {
                    var scope = _scope;
                    var set = new ScopeSet(_scope);
                    var buffer = new Metadata[_depth];

                    do
                    {
                        for (var index = 1; 0 < index; index = scope.GetNextType(index))
                        {
                            // Skip named registrations
                            if (set.Contains(in scope[index].Internal.Contract))
                                continue;

                            var _iterator = new NewEnumerator(scope, index, buffer);

                            while (_iterator.MoveNext())
                            {
                                if (!set.Add(in _iterator)) continue;

                                if (null == _iterator.Internal.Manager || RegistrationCategory.Internal > _iterator.Internal.Manager.Category)
                                    continue;

                                yield return _iterator.Registration;
                            }
                        }
                    }
                    while (null != (scope = scope.Next!));

                    // TODO: Broadcast cache
                    //_cache = set._data;
                }
                else
                {
                    var count = _cache.Count();
                    for (var index = 1; index <= count; index++)
                    {
                        var record = _cache[index];
                        var manager = _scope[in record].Internal.Manager;

                        if (null == manager || RegistrationCategory.Internal > manager.Category)
                            continue;

                        yield return _scope[in record].Registration;
                    }
                }
            }
        }

        #endregion
    }
}
