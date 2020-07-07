using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Lifetime;
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
                ScopeRegistrations[] registrations = 
                    Hierarchy().Where(scope => START_DATA <= scope._registryCount)
                               .Select(scope => new ScopeRegistrations(scope._registryCount, scope._registryData))
                               .ToArray();

                return new RegistrationsSet((ContainerLifetimeManager)_registryData[START_INDEX].Manager, registrations);
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

        /// <summary>
        /// Internal enumerable wrapper
        /// </summary>
        [DebuggerDisplay("Registrations")]
        private class RegistrationsSet : IEnumerable<ContainerRegistration>
        {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private int _prime;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ScopeRegistrations[] _registrations;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ContainerLifetimeManager _manager;

            #endregion


            #region Constructors

            /// <summary>
            /// Constructor for the enumerator
            /// </summary>
            /// <param name="scope"></param>
            public RegistrationsSet(ContainerLifetimeManager manager, ScopeRegistrations[] registrations)
            {
                _manager = manager;
                _registrations = registrations;
                _prime = Prime.IndexOf(_registrations.Sum(scope => scope.Data.Length));
            }

            #endregion


            #region IEnumerable 

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <inheritdoc />
            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                throw new NotImplementedException();
                //// Built-in registrations
                //yield return new ContainerRegistration(typeof(IUnityContainer),      _manager);
                //yield return new ContainerRegistration(typeof(IServiceProvider),     _manager);
                //yield return new ContainerRegistration(typeof(IUnityContainerAsync), _manager);

                //if (0 == _registrations.Length) yield break;

                //var count = 1;
                //var size = Prime.Numbers[_prime];
                //Span<SetStruct> set = stackalloc SetStruct[size];

                //// Explicit registrations
                //for (var level = 0; level < _registrations.Length; level++)
                //{
                //    var scope = _registrations[level];

                //    // Iterate registrations
                //    for (var registration = START_DATA; registration <= scope.Count; registration++)
                //    {
                //        // Skip internal registrations
                //        var manager = scope.Data[registration].Manager;
                //        if (RegistrationType.Internal == manager.RegistrationType) continue;

                //        var hashCode = scope.Data[registration].HashCode;
                //        var targetBucket = hashCode % size;
                //        for (var i = set[targetBucket].Bucket; i > 0; i = set[i].Next)
                //        {
                //            //var registry = _registrations[set[i].Registry];
                //            //var value = registry[set[i].Index];

                //            //var candidate = .Data scope.Data[i];
                //            //if (candidate.Type != type || candidate.Name != name) continue;

                //            //ReplaceManager(ref candidate, manager);
                //            //return;
                //        }

                //        set[count].Registry      = level;
                //        set[count].Index         = registration;
                //        set[count].Next          = set[targetBucket].Bucket;
                //        set[targetBucket].Bucket = count++;

                //        yield return new ContainerRegistration(scope.Data[registration].Type, scope.Data[registration].Name, (LifetimeManager)manager);
                //    }
                //}
            }

            #endregion

            private struct SetStruct
            {
                public int Next;
                public int Bucket;

                public int Index;
                public int Registry;
            }
        }

        private readonly struct ScopeRegistrations
        {
            public readonly int Count;
            public readonly Registry[] Data;

            public ScopeRegistrations(int count, Registry[] data)
            {
                Count = count;
                Data = data;
            }
        }
    }
}
