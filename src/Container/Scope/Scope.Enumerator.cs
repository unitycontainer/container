using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Storage;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        /// <summary>
        /// Method that creates <see cref="IUnityContainer.Registrations"/> enumerator
        /// </summary>
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                return (null == Parent) 
                    ? (IEnumerable<ContainerRegistration>)new RootRegistrationsEnumerator(this)
                    : new ChildRegistrationsEnumerator(this);
            }
        }

        /// <summary>
        /// Method that creates <see cref="IUnityContainer.Registrations"/> enumerator
        /// </summary>
        protected IEnumerable<ContainerScope> Hierarchy()
        {
            for (ContainerScope? scope = this; null != scope; scope = scope.Parent)
            {
                yield return scope;
            }
        }


        #region Root Scope Enumerator

        /// <summary>
        /// Root container enumerable wrapper
        /// </summary>
        [DebuggerDisplay("Registrations")]
        private class RootRegistrationsEnumerator : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected int _length;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected Contract[] _identity;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected Registry[] _registry;

            #endregion


            #region Constructor

            public RootRegistrationsEnumerator(ContainerScope root)
            {
                _length   = root._registryCount;
                _identity = root._contractData;
                _registry = root._registryData;
            }

            #endregion


            #region IEnumerable

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                ContainerRegistration registration = new ContainerRegistration();

                for (var i = START_INDEX; i <= _length; i++)
                {
                    var manager = _registry[i].Manager;
                    
                    if (RegistrationType.Internal == manager.RegistrationType) 
                        continue;

                    registration._type = _registry[i].Type;
                    registration._manager = manager;
                    registration._name = _identity[_registry[i].Identity].Name;

                    yield return registration;
                }
            }

            #endregion
        }

        #endregion


        #region Child Scope Enumerator

        /// <summary>
        /// Internal enumerable wrapper
        /// </summary>
        [DebuggerDisplay("Registrations")]
        private class ChildRegistrationsEnumerator : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly int _prime;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly ScopeData[] _registrations;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly ContainerScope _scope;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            SetsData[]? _cache;

            #endregion


            #region Constructors

            /// <summary>
            /// Constructor for the enumerator
            /// </summary>
            /// <param name="scope"></param>
            public ChildRegistrationsEnumerator(ContainerScope scope)
            {
                _registrations = scope
                    .Hierarchy()
                    .Where(scope => START_COUNT < scope._registryCount)
                    .Select(scope => new ScopeData(scope._registryCount, scope._registryData, scope._contractData))
                    .ToArray();

                _scope = scope;
                _prime = Prime.IndexOf(_registrations.Sum(scope => scope.Count - START_COUNT) + 1);
            }

            #endregion


            #region IEnumerable 

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <inheritdoc />
            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                ContainerRegistration registration = new ContainerRegistration();

                // Built-in types
                for(var i = START_INDEX; i <= START_COUNT; i++)
                {
                    registration._type    = _scope._registryData[i].Type;
                    registration._manager = _scope._registryData[i].Manager;
                    
                    yield return registration;
                };

                if (null == _cache)
                {
                    if (0 == _registrations.Length) yield break;

                    //var count = 1;
                    var size = Prime.Numbers[_prime];

                    var meta = new Metadata[size];
                    var data = new SetsData[size];

                    // Explicit registrations
                    for (var level = 0; level < _registrations.Length; level++)
                    {
                        var length = _registrations[level].Count;
                        var registry = _registrations[level].Registry;

                        // Iterate registrations at this level
                        for (var index = START_DATA; index <= length; index++)
                        {
                            var contract = registry[index];

                            // Skip internal registrations
                            if (RegistrationType.Internal == contract.Manager.RegistrationType) 
                                continue;

                            // Check if already served
                            var targetBucket = contract.Hash % size;
                            var position = meta[targetBucket].Position;
                            var location = data[position].Registry;

                            while (position > 0)
                            {
                                var entry = _registrations[location].Registry[data[position].Index];

                                if (contract.Type == entry.Type && 
                                    contract.Identity == entry.Identity) break;

                                position = meta[position].Next;
                            }

                            // Add new registration
                            if (0 == position)
                            {
                                var count = data[0].Index + 1;
                                data[0].Index = count;

                                data[count].Registry = level;
                                data[count].Index = index;
                                data[0].Index = count;

                                meta[count].Next = meta[targetBucket].Position;
                                meta[targetBucket].Position = count;

                                registration._type    = contract.Type;
                                registration._name    = _registrations[location].Identity[contract.Identity].Name;
                                registration._manager = contract.Manager;

                                yield return registration;
                            }
                        }
                    }

                    _cache = data;
                }
                else
                {
                    var length = _cache[0].Index;
                    for (var i = START_INDEX; i <= length; i++)
                    {
                        var index    = _cache[i].Index;
                        var offset   = _cache[i].Registry;
                        var registry = _registrations[offset].Registry;
                        var metadata = _registrations[offset].Identity;
                        var identity = registry[index].Identity;

                        registration._type    = registry[index].Type;
                        registration._manager = registry[index].Manager;
                        registration._name    = metadata[identity].Name;

                        yield return registration;
                    }
                }
            }

            #endregion

            [DebuggerDisplay("Registry = {Registry}, Index = {Index}")]
            private struct SetsData
            {
                public int Index;
                public int Registry;
            }

            [DebuggerDisplay("Count = {Count}")]
            private readonly struct ScopeData
            {
                public readonly int        Count;
                public readonly Registry[] Registry;
                public readonly Contract[] Identity;

                public ScopeData(int count, Registry[] registry, Contract[] identity)
                {
                    Count    = count;
                    Registry = registry;
                    Identity = identity;
                }
            }

        }

        #endregion
    }
}
