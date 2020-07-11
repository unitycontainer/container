using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public partial class ContainerScope : IDisposable
    {
        #region Public Members

        /// <summary>
        /// Parent scope
        /// </summary>
        public readonly ContainerScope? Parent;

        /// <summary>
        /// Owner container
        /// </summary>
        public readonly UnityContainer Container;

        /// <summary>
        /// Version of scope
        /// </summary>
        /// <remarks>
        /// Scope version is increased every time registrations change
        /// in any way
        /// </remarks>
        public int Version => _version;

        /// <summary>
        /// Registration count
        /// </summary>
        public int Count => _registrations - (START_DATA - START_INDEX);

        public ICollection<IDisposable> Disposables => _lifetimes;

        #endregion


        #region Registrations

        public bool IsRegistered(Type type)
        {
            var scope = this;

            do
            {
                var bucket = (uint)type.GetHashCode() % scope._registryMeta.Length;
                var position = scope._registryMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (candidate.Identity == 0 && candidate.Type == type)
                        return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = scope.Parent) != null);

            return false;
        }

        public bool IsRegistered(Type type, string name)
        {
            var scope = this;

            do
            {
                if (0 == scope._contracts) continue;

                var hash = type.GetHashCode(name);
                var bucket = hash % scope._registryMeta.Length;
                var position = scope._registryMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (candidate.Type == type && name == scope._contractData[candidate.Identity].Name) 
                        return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = scope.Parent) != null);

            return false;
        }

        public virtual void Register(ref RegistrationData data)
        {
            if (null == data.Name)
                RegisterAnonymous(ref data);
            else
                RegisterContracts(ref data);
        }

        #endregion


        #region Child Scope

        /// <summary>
        /// Creates new scope for child container
        /// </summary>
        /// <param name="container"><see cref="UnityContainer"/> that owns the scope</param>
        /// <returns>New scope associated with the container</returns>
        public virtual ContainerScope CreateChildScope(UnityContainer container)
            => new ContainerScope(container);

        #endregion


        #region IDisposable

        /// <summary>
        /// Dispose current scope
        /// </summary>
        public void Dispose() => Dispose(true);

        #endregion
    }
}
