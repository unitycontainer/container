using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope
        {
            /// <summary>
            /// Method that creates <see cref="IUnityContainer.Registrations"/> enumerator
            /// </summary>
            public IEnumerable<ContainerRegistration> Registrations => new RegistrationsSet(this);


            /// <summary>
            /// Internal enumerable wrapper
            /// </summary>
            [DebuggerDisplay("Registrations")]
            private class RegistrationsSet : IEnumerable<ContainerRegistration>
            {
                #region Fields

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private ContainerScope _scope;

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private int _prime;

                #endregion


                #region Constructors

                /// <summary>
                /// Constructor for the enumerator
                /// </summary>
                /// <param name="scope"></param>
                public RegistrationsSet(ContainerScope scope)
                {
                    _scope = scope;
                    _prime = scope.Container._root._scope._registryPrime + scope.Container._level;
                }

                #endregion


                #region IEnumerable 

                /// <inheritdoc />
                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                /// <inheritdoc />
                public IEnumerator<ContainerRegistration> GetEnumerator()
                {
                    var lifetime = (LifetimeManager)_scope._registryData[0].Manager;
                    var set = new QuickSet<Type>(_prime);
                    
                    // Built-in registrations
                    yield return new ContainerRegistration(typeof(IUnityContainer),      lifetime);
                    yield return new ContainerRegistration(typeof(IServiceProvider),     lifetime);
                    yield return new ContainerRegistration(typeof(IUnityContainerAsync), lifetime);
                    
                    // Explicit registrations
                    for (ContainerScope? scope = _scope; null != scope; scope = scope.Parent)
                    {
                        // Skip if no user registrations
                        if (START_DATA > scope._registryCount) continue;

                        // Iterate registrations
                        for (var i = START_DATA; i <= scope._registryCount; i++)
                        {
                            var entry = scope._registryData[i];

                            if (RegistrationType.Internal == entry.Manager.RegistrationType ||
                                !set.Add(entry.Type, entry.HashCode)) continue;

                            yield return new ContainerRegistration(entry.Type, entry.Name, (LifetimeManager)entry.Manager);
                        }
                    }
                }

                #endregion
            }
        }
    }
}
