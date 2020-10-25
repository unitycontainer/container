using System;
using System.Collections.Generic;
using Unity.Storage;

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
                if (null == _cache || _scope.Version != _cache.Version())
                {
                    var scope = _scope;
                    var set = new Container.Scope.ScopeSet(_scope);
                    var buffer = new Metadata[_ancestry.Length];

                    do
                    {
                        for (var index = 1; 0 < index; index = scope.GetNextType(index))
                        {
                            // Skip named registrations
                            if (set.Contains(in scope[index].Internal.Contract))
                                continue;

                            var _iterator = new Container.Scope.Enumerator(scope, index, buffer);

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
                        var scope = _ancestry[record.Location]._scope;
                        var manager = scope[record.Position].Internal.Manager;

                        if (null == manager || RegistrationCategory.Internal > manager.Category)
                            continue;

                        yield return scope[record.Position].Registration;
                    }
                }
            }
        }

        #endregion
    }
}
