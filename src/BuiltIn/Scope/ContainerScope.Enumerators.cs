using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        /// <summary>
        /// Method that creates <see cref="IUnityContainer.Registrations"/> enumerator
        /// </summary>
        public override IEnumerable<ContainerRegistration> Registrations 
            => (null == _next)
            ? (IEnumerable<ContainerRegistration>)new SingleScopeEnumerator(GetHashCode(), this)
            : new MultiScopeEnumerator(GetHashCode(), this);

        #region Single Scope Enumerator

        /// <summary>
        /// Root container enumerable wrapper
        /// </summary>
        [DebuggerDisplay("Registrations")]
        private class SingleScopeEnumerator : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected int _hashCode;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected int _length;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected NameInfo[]? _identity;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected Registration[] _registry;

            #endregion


            #region Constructor

            public SingleScopeEnumerator(int hash, ContainerScope root)
            {
                _hashCode = hash;
                _length   = root._registryCount;
                _identity = root._namesData;
                _registry = root._registryData;
            }

            #endregion


            #region IEnumerable

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                for (var i = START_INDEX; i <= _length; i++)
                {
                    var manager = (LifetimeManager)_registry[i].Manager;
                    
                    if (RegistrationType.Internal == manager.RegistrationType) 
                        continue;

                    yield return new ContainerRegistration(in _registry[i].Contract, manager);
                }
            }

            #endregion


            #region Object

            public override int GetHashCode() => _hashCode;

            #endregion
        }

        #endregion


        #region Multi Scope Enumerator

        /// <summary>
        /// Internal enumerable wrapper
        /// </summary>
        [DebuggerDisplay("Registrations")]
        private class MultiScopeEnumerator : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected int _hashCode;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly int _prime;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly ScopeInfo[] _registrations;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly ContainerScope _scope;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ScopeData[]? _cache;


            #endregion


            #region Constructors

            /// <summary>
            /// Constructor for the enumerator
            /// </summary>
            /// <param name="scope"></param>
            public MultiScopeEnumerator(int hash, ContainerScope scope)
            {
                _hashCode = hash;

                _registrations = scope
                    .Hierarchy()
                    .Where(scope => START_DATA <= scope._registryCount)
                    .Select(scope => new ScopeInfo(scope._registryCount, scope._registryData))
                    .ToArray();

                _scope = scope;
                _prime = Prime.IndexOf(_registrations.Sum(scope => scope.Count - (START_DATA - START_INDEX)) + 1);
            }

            #endregion


            #region IEnumerable 

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <inheritdoc />
            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                // Registered types
                if (null == _cache)
                {
                    if (0 == _registrations.Length) yield break;

                    //var count = 1;
                    var size = Prime.Numbers[_prime];

                    var meta = new Metadata[size];
                    var data = new ScopeData[size];

                    // Explicit registrations
                    for (var level = 0; level < _registrations.Length; level++)
                    {
                        var length = _registrations[level].Count;
                        var registry = _registrations[level].Registry;

                        // Iterate registrations at this level
                        for (var index = START_INDEX; index <= length; index++)
                        {
                            var registration = registry[index];

                            // Skip internal registrations
                            if (RegistrationType.Internal == registration.Manager.RegistrationType) 
                                continue;

                            // Check if already served
                            var targetBucket = registration.Hash % size;
                            var position = meta[targetBucket].Position;
                            var location = data[position].Registry;

                            while (position > 0)
                            {
                                var entry = _registrations[location].Registry[data[position].Index];

                                if (registration.Contract.Type == entry.Contract.Type && 
                                    ReferenceEquals( registration.Contract.Name, entry.Contract.Name)) break;

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

                                yield return new ContainerRegistration(in registration.Contract, (LifetimeManager)registration.Manager);
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

                        yield return new ContainerRegistration(in registry[index].Contract, (LifetimeManager)registry[index].Manager);
                    }
                }
            }

            #endregion


            #region Object

            public override int GetHashCode() => _hashCode;

            #endregion


            #region Nested Types

            [DebuggerDisplay("Registry = {Registry}, Index = {Index}")]
            private struct ScopeData
            {
                public int Index;
                public int Registry;
            }

            [DebuggerDisplay("Count = {Count}")]
            private readonly struct ScopeInfo
            {
                public readonly int        Count;
                public readonly Registration[] Registry;

                public ScopeInfo(int count, Registration[] registry)
                {
                    Count    = count;
                    Registry = registry;
                }
            }

            #endregion
        }

        #endregion
    }
}
