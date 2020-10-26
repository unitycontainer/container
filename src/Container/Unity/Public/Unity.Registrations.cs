using System;
using System.Collections.Generic;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private WeakReference<Metadata[]>? _cache;

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
                if (null != _cache && _cache.TryGetTarget(out var recording) && _scope.Version == recording.Version())
                {
                    var count = recording.Count();
                    for (var i = 1; i <= count; i++) yield return _scope[in recording[i]].Registration;
                }
                else
                { 
                    var set = new RegistrationSet(_scope);
                    var enumerator = new Enumerator(this);

                    while (enumerator.MoveNext())
                    {
                        var manager = enumerator.Manager;

                        if (null == manager || RegistrationCategory.Internal > manager.Category || 
                            !set.Add(in enumerator)) continue;

                        yield return enumerator.Registration;
                    }
                    
                    _cache = new WeakReference<Metadata[]>(set.GetRecording());
                }
            }
        }

        #endregion
    }
}
