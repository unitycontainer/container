﻿using System;
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
        #region Constants

        private const string INVALID_ENUMERATOR = "Collection was modified; enumeration operation may not execute";

        #endregion


        #region Single Scope Enumerator

        /// <summary>
        /// Single container enumerable
        /// </summary>
        [DebuggerDisplay("Version = { _version }")]
        private class SingleScopeEnumerator : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly int _version;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly Scope _scope;

            #endregion


            #region Constructor

            public SingleScopeEnumerator(UnityContainer container)
            {
                _version = container._scope.Version;
                _scope = container._scope;
            }

            #endregion


            #region IEnumerable

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                var memory = _scope.Memory;

                for (var i = 0; i < memory.Length; i++)
                {
                    if (_version != _scope.Version)
                        throw new InvalidOperationException(INVALID_ENUMERATOR);

                    var entry = memory.Span[i];

                    if (null == entry.Internal.Manager || RegistrationCategory.Internal >= entry.Internal.Manager.Category)
                        continue;

                    yield return entry.Registration;
                }
            }

            #endregion
        }

        #endregion


        #region Multi Scope Enumerator

        /// <summary>
        /// Single container enumerable
        /// </summary>
        [DebuggerDisplay("Version = { _version }")]
        private class MultiScopeEnumerator : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly int _length;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly int _version;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly IList<Scope> _scopes;

            #endregion


            #region Constructor

            public MultiScopeEnumerator(IList<Scope> scopes)
            {
                _scopes = scopes;
                _version = scopes[0].Version;
                _length = Prime.GetPrime(
                    scopes.Select(s => s.Count)
                          .Sum());
            }

            #endregion


            #region IEnumerable

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                var root = _scopes[0];
                var meta = new Metadata[_length];
                var data = new Metadata[_length];
                var count = 1;

                for (var level = 0; level < _scopes.Count; level++)
                { 
                    var scope  = _scopes[level];
                    var memory = scope.Memory;
                    for (var i = 0; i < memory.Length; i++)
                    {
                        if (_version != root.Version)
                            throw new InvalidOperationException(INVALID_ENUMERATOR);

                        var info = memory.Span[i];

                        if (null == info.Internal.Manager || RegistrationCategory.Internal >= info.Internal.Manager.Category)
                                continue;

                        // Check if already served
                        var targetBucket = (uint)info.Internal.Contract.HashCode % _length;
                        var position = meta[targetBucket].Position;

                        while (position > 0)
                        {
                            var entry = _scopes[data[position].Location].Memory.Span[data[position].Position];

                            if (info.Internal.Contract.Type == entry.Internal.Contract.Type &&
                                ReferenceEquals(info.Internal.Contract.Name, entry.Internal.Contract.Name)) 
                                break; // Skip, already enumerated

                            position = meta[position].Location;
                        }

                        // Add new registration
                        if (0 == position)
                        {
                            data[count].Location = level;
                            data[count].Position = i;
                            data[0].Position = count;

                            meta[count].Location = meta[targetBucket].Position;
                            meta[targetBucket].Position = count++;

                            yield return info.Registration;
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
