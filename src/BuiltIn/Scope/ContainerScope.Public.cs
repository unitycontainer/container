using System;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Public Members

        /// <inheritdoc />
        public override int Contracts => _registryCount;

        /// <inheritdoc />
        public override int Names => _namesCount;

        #endregion


        #region Add

        /// <inheritdoc />
        public override void Add(RegistrationManager manager, params Type[] registerAs)
        {
            // Expand if required
            var required = _registryCount + registerAs.Length;
            if (required >= _registryMeta.MaxIndex()) ExpandRegistry(required);

            // Iterate and register types
            foreach (var type in registerAs)
            {
                // Skip invalid types
                if (null == type) continue;

                // Check for existing
                var hash = (uint)type.GetHashCode();
                ref var bucket = ref _registryMeta[hash % _registryMeta.Length];
                var position = bucket.Position;
                while (position > 0)
                {
                    ref var candidate = ref _registryData[position];
                    if (candidate.Contract.Type == type &&
                        candidate.Contract.Name == null)
                    {
                        // Found existing
                        ReplaceManager(ref candidate, manager);
                        break;
                    }

                    position = _registryMeta[position].Next;
                }

                // Add new registration
                if (0 == position)
                {
                    _registryData[++_registryCount] = new Registration(hash, type, manager);
                    _registryMeta[_registryCount].Next = bucket.Position;
                    bucket.Position = _registryCount;
                    _version += 1;
                }
            }
        }

        /// <inheritdoc />
        public override void Add(in ReadOnlySpan<RegistrationDescriptor> data)
        {
            //lock (_syncRoot)
            _registryLock.EnterWriteLock();
            {
                for (var i = 0; data.Length > i; i++)
                {
                    ref readonly RegistrationDescriptor descriptor = ref data[i];

                    if (null == descriptor.Name)
                    {
                        Add(descriptor.Manager, descriptor.RegisterAs);
                    }
                    else
                    {
                        var nameInfo = GetNameInfo(descriptor.Name);

                        Add(in descriptor, ref nameInfo);
                    }

                }
            }
            _registryLock.ExitWriteLock();
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        public override bool Contains(Type type)
        {
            var scope = this;

            do
            {
                var bucket = (uint)type.GetHashCode() % scope._registryMeta.Length;
                var position = scope._registryMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (null == candidate.Contract.Name && candidate.Contract.Type == type)
                        return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = (ContainerScope?)scope._next) != null);

            return false;
        }

        /// <inheritdoc />
        public override bool Contains(Type type, string name)
        {
            var scope = this;

            do
            {
                if (0 == scope._namesCount) continue;

                var hash = type.GetHashCode(name);
                var bucket = hash % scope._registryMeta.Length;
                var position = scope._registryMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (candidate.Contract.Type == type && candidate.Contract.Name == name) 
                        return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = (ContainerScope?)scope._next) != null);

            return false;
        }


        #endregion


        #region Child Scope

        /// <inheritdoc />
        public override Scope CreateChildScope() => new ContainerScope(this);

        #endregion
    }
}
