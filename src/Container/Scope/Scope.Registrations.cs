using System;
using System.Diagnostics;
using System.Linq;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        #region IEnumerable

        public Func<Enumerator<ContainerRegistration>> RegistrationsEnumeratorFactory
        {
            get
            {
                // Create snapshot of containers registrations
                var hierarchy = from scope in Hierarchy()
                                where this == scope || START_DATA <= scope._registrations
                                select new ScopeInfo(scope._registrations, scope._registryData, scope._contractData);

                var registrations = hierarchy.ToArray();

                // Single level enumerator
                if (1 == registrations.Length) return () => GetEnumerator(ref registrations[0]);

                // Multi-level enumerator
                ScopeData[]? cache = null;
                var prime = Prime.IndexOf(registrations.Sum(scope => scope.Count - (START_DATA - START_INDEX)) + START_DATA);
                var size = Prime.Numbers[prime];

                return () => 
                { 
                    return null == cache 
                        // Build cache on first run
                        ? GetEnumerator(registrations, size, (data) => { if (null == cache) cache = data; })
                        // Reuse on subsequent runs
                        : GetEnumerator(registrations, cache); 
                };
            }
        }

        private Enumerator<ContainerRegistration> GetEnumerator(ref ScopeInfo scope)
        {
            var count    = scope.Count;
            var registry = scope.Registry;
            var contract = scope.Identity;
            var position = 0;

            return new Enumerator<ContainerRegistration>((out ContainerRegistration current) =>
            {
                while (++position <= count)
                {
                    var manager = (LifetimeManager)registry[position].Manager;

                    if (RegistrationType.Internal == manager.RegistrationType)
                        continue;

                    current = new ContainerRegistration(
                        registry[position].Type,
                        contract?[registry[position].Identity].Name,
                        manager);

                    return true;
                }

                return false;
            });
        }

        private Enumerator<ContainerRegistration> GetEnumerator(ScopeInfo[] hierarchy, ScopeData[] data)
        {
            var index = START_INDEX;

            return new Enumerator<ContainerRegistration>((out ContainerRegistration current) =>
            {
                while (index < data.Length)
                {
                    ref var scope = ref data[index++];
                    ref var registration = ref hierarchy[scope.Registry].Registry[scope.Index];

                    current = new ContainerRegistration(registration.Type,
                        hierarchy[scope.Registry].Identity?[registration.Identity].Name,
                        (LifetimeManager)registration.Manager);

                    return true;
                }

                return false;
            });
        }

        private Enumerator<ContainerRegistration> GetEnumerator(ScopeInfo[] hierarchy, int size, Action<ScopeData[]> set)
        {
            var meta = new Metadata[size];
            var data = new ScopeData[size];
            var index = START_INDEX;
            int count = START_INDEX;
            var level = 0;
            var setCache = set;

            return new Enumerator<ContainerRegistration>((out ContainerRegistration current) =>
            {
                // Hierarchical registrations
                while (hierarchy.Length > level)
                { 
                    ref var scope = ref hierarchy[level];
                    while (index <= scope.Count)
                    {
                        ref var registration = ref scope.Registry[index];

                        // Skip internal registrations
                        if (RegistrationType.Internal == registration.Manager.RegistrationType)
                        {
                            index++;
                            continue;
                        }

                        // Check if already served
                        var bucket   = registration.Hash % size;
                        var position = meta[bucket].Position;
                        var location = data[position].Registry;

                        while (position > 0)
                        {
                            ref var entry = ref hierarchy[location].Registry[data[position].Index];

                            if (registration.Type     == entry.Type &&
                                registration.Identity == entry.Identity) break;

                            position = meta[position].Next;
                        }

                        // Add new registration
                        if (0 == position)
                        {
                            data[count] = new ScopeData(index, level);
                            meta[count].Next = meta[bucket].Position;
                            meta[bucket].Position = count++;

                            current = new ContainerRegistration(registration.Type,
                                hierarchy[location].Identity?[registration.Identity].Name,
                                (LifetimeManager)registration.Manager);

                            index++;
                            return true;
                        }

                        index++;
                    }

                    // Go to next container
                    level++;
                    index = START_DATA;
                }

                // Set cache
                if (null != setCache)
                {
                    Array.Resize(ref data, count);
                    setCache(data);
                    setCache = null;
                }

                return false;
            });
        }

        #endregion


        [DebuggerDisplay("Registry = {Registry}, Index = {Index}")]
        private readonly struct ScopeData
        {
            public readonly int Index;
            public readonly int Registry;
            public ScopeData(int index, int registry)
            {
                Index = index;
                Registry = registry;
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        private readonly struct ScopeInfo
        {
            public readonly int Count;
            public readonly Registry[] Registry;
            public readonly Contract[]? Identity;

            public ScopeInfo(int count, Registry[] registry, Contract[]? identity)
            {
                Count = count;
                Registry = registry;
                Identity = identity;
            }
        }
    }
}
