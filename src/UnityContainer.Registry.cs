using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Factories;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        private const int CollisionsCutPoint = 5;
        internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));

        #endregion


        #region Fields

        private Registry<NamedType, InternalRegistration> _registry;
        private Metadata _metadata;

        #endregion


        #region Defaults

        internal InternalRegistration Defaults => _root._registry.Entries[0].Value;

        #endregion


        #region Registrations Manipulation

        private void InitializeRootRegistry()
        {
            Register = AddOrReplace;

            // Create Registry
            _metadata = new Metadata();
            _registry = new Registry<NamedType, InternalRegistration>();

            // Default Policies 
            _registry.Set(new InternalRegistration());

            // Register Container as IUnityContainer & IUnityContainerAsync
            var container = new ContainerRegistration(typeof(UnityContainer), new ContainerLifetimeManager());
            _registry.Set(typeof(IUnityContainer), null, container);
            _registry.Set(typeof(IUnityContainerAsync), null, container);

            // Func<> Factory
            var funcBuiltInFctory = new InternalRegistration();
            funcBuiltInFctory.Set(typeof(LifetimeManager), new PerResolveLifetimeManager());
            funcBuiltInFctory.Set(typeof(ResolveDelegateFactory), FuncResolver.Factory);

            // Setup Built-in Factories
            _registry.Set(typeof(Func<>),        funcBuiltInFctory);
            _registry.Set(typeof(Lazy<>),        new InternalRegistration(typeof(ResolveDelegateFactory), LazyResolver.Factory));
            _registry.Set(typeof(IEnumerable<>), new InternalRegistration(typeof(ResolveDelegateFactory), EnumerableResolver.Factory));

            // TODO: requires optimization
            container.BuildChain = new[] { new LifetimeStrategy() };
        }

        private InternalRegistration InitAndAdd(Type type, string name, InternalRegistration registration)
        {
            lock (_syncRoot)
            {
                if (Register == InitAndAdd)
                {
                    _registry = new Registry<NamedType, InternalRegistration>();
                    _metadata = new Metadata();

                    Register = AddOrReplace;
                }
            }

            return Register(type, name, registration);
        }

        private InternalRegistration AddOrReplace(Type type, string name, InternalRegistration registration)
        {
            var hashCode = NamedType.GetHashCode(type, name) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registry.Buckets.Length;
            var collisions = 0;

            lock (_syncRoot)
            {
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    // Replace registration
                    var existing = candidate.Value;

                    candidate.Value = registration;
                    candidate.Value.AddRef();

                    return existing;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry<NamedType, InternalRegistration>(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Key.Type = type;
                entry.Key.Name = name;
                entry.Value = registration;
                entry.Value.AddRef();

                var position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
                _metadata.Add(type, position);

                return null;
            }
        }

        private InternalRegistration GetOrAdd(int hashCode, Type type, string name, InternalRegistration factory)
        {

            lock (_syncRoot)
            {
                var collisions = 0;
                var targetBucket = hashCode % _registry.Buckets.Length;

                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    return candidate.Value;
                }

                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry<NamedType, InternalRegistration>(_registry);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Key.Type = type;
                entry.Key.Name = name;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = CreateRegistration(type, name, factory);
                entry.Value.AddRef();
                _registry.Buckets[targetBucket] = _registry.Count++;

                return entry.Value;
            }
        }

        #endregion
    }

    #region Registry Query Methods

    internal static class RegistryExtensions
    {
        #region Get

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InternalRegistration Get(this Registry<NamedType, InternalRegistration> registry, Type type, string name)
        {
            var hashCode = NamedType.GetHashCode(type, name);
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return entry.Value;
            }

            return null;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InternalRegistration Get(this Registry<NamedType, InternalRegistration> registry, ref NamedType key)
        {
            var hashCode = key.GetHashCode();
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != key.Type) continue;
                return entry.Value;
            }

            return null;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static InternalRegistration Get(this Registry<NamedType, InternalRegistration> registry, int hashCode, Type type)
        {
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return entry.Value;
            }

            return null;
        }

        #endregion


        #region Set

        internal static void Set(this Registry<NamedType, InternalRegistration> registry, InternalRegistration registration)
        {
            var targetBucket = 0 % registry.Buckets.Length;

            ref var entry = ref registry.Entries[0];
            entry.Next = registry.Buckets[targetBucket];
            entry.Value = registration;
            registry.Buckets[targetBucket] = 0;

            if (0 == registry.Count) registry.Count++;
        }

        internal static void Set(this Registry<NamedType, InternalRegistration> registry, Type type, InternalRegistration registration)
        {
            var hashCode = type.GetHashCode() & UnityContainer.HashMask;
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.Key.Type != type) continue;
                candidate.Value = registration;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Value = registration;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        internal static void Set(this Registry<NamedType, InternalRegistration> registry, Type type, string name, InternalRegistration registration)
        {
            var hashCode = NamedType.GetHashCode(type, name) & UnityContainer.HashMask;
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.Key.Type != type) continue;
                candidate.Value = registration;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Key.Name = name;
            entry.Value = registration;
            registry.Buckets[targetBucket] = registry.Count++;
        }


        #endregion


        #region Contains

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Contains(this Registry<NamedType, InternalRegistration> registry, Type type, string name)
        {
            var hashCode = NamedType.GetHashCode(type, name);
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return true;
            }

            return false;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Contains(this Registry<NamedType, InternalRegistration> registry, ref NamedType key)
        {
            var hashCode = key.GetHashCode();
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != key.Type) continue;
                return true;
            }

            return false;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Contains(this Registry<NamedType, InternalRegistration> registry, int hashCode, Type type)
        {
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return true;
            }

            return false;
        }

        #endregion
    }

    #endregion
}
