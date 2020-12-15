using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        [DebuggerDisplay("Contracts = {_scope.Count}, Version = {_scope.Version}")]
        [DebuggerTypeProxy(typeof(RegistrationsDebugProxy))]
        private class UnityRegistrations : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private int _version;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Metadata[]? _data;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            private readonly Scope _scope;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly UnityContainer _container;

            #endregion


            #region Constructors

            public UnityRegistrations(UnityContainer container, Scope scope)
            {
                _scope = scope;
                _version = scope.Version;
                _container = container;
            }

            #endregion


            #region Public API

            public int Version() => _version;

            #endregion


            #region IEnumerable

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                if (null != _data)
                {
                    var count = _data.Count();
                    for (var i = 1; i <= count; i++) yield return _scope[in _data[i]].Registration;
                }
                else
                {
                    var set = new RegistrationSet(_scope);
                    var enumerator = new Enumerator(_container);

                    while (enumerator.MoveNext())
                    {
                        var manager = enumerator.Manager;

                        if (manager is null || RegistrationCategory.Internal > manager.Category ||
                            !set.Add(in enumerator)) continue;

                        yield return enumerator.Registration;
                    }

                    _data = set.GetRecording();
                }
            }

            #endregion


            #region Nested 

            [DebuggerDisplay("Contracts = {Items.Length}, Version = {Version}")]
            private class RegistrationsDebugProxy
            {
                public RegistrationsDebugProxy(UnityRegistrations registrations)
                {
                    Items = registrations.ToArray();
                    Version = registrations.Version();
                }

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public readonly int Version;

                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public ContainerRegistration[] Items { get; }
            }

            #endregion
        }
    }
}
