using System;
using System.Runtime.InteropServices;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Add Default

        private void AddDefault(in RegistrationDescriptor descriptor)
        {
            if (0 == descriptor.RegisterAs.Length)
            {
                var type = descriptor.Manager.Category switch
                {
                    RegistrationCategory.Type => descriptor.Manager.Type,

                    // TODO: Proper error handling
                    RegistrationCategory.Instance when null == descriptor.Manager.Data
                        => throw new ArgumentException($"Registration Manager {descriptor.Manager} is invalid", "manager"),

                    RegistrationCategory.Instance => descriptor.Manager.Instance!.GetType(),

                    _ => throw new ArgumentException($"Registration Manager {descriptor.Manager} is invalid", "manager"),
                };

                if (null == type) return;

                Add(type, descriptor.Manager);
            }
            else
            {
                // Anonymous contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    // TODO: Proper error handling
                    if (null == type) continue;

                    Add(type, descriptor.Manager);
                }
            }
        }

        #endregion


        #region Add Contract

        private void AddContract(in RegistrationDescriptor descriptor)
        {
            var name = descriptor.Name!;
            var nameHash = name.GetHashCode();
            
            ContractUnion union = new ContractUnion(descriptor.Name!);

            if (0 == descriptor.RegisterAs.Length)
            {
                // TODO: ???
                var type = descriptor.Manager.Category switch
                {
                    RegistrationCategory.Type => descriptor.Manager.Type,
                    RegistrationCategory.Instance => descriptor.Manager.Instance?.GetType(),
                    _ => throw new NotImplementedException(),
                };

                if (null == type) return;

                union.AsStruct.Type = type;
                union.AsStruct.HashCode = Contract.GetHashCode(type.GetHashCode(), nameHash);

                Add(in union.Contract, descriptor.Manager);
            }
            else
            {
                // Register contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    if (null == type) continue;

                    union.AsStruct.Type = type;
                    union.AsStruct.HashCode = Contract.GetHashCode(type.GetHashCode(), nameHash);
                    
                    // TODO: Add dealing with replacement
                    Add(type, name, nameHash, descriptor.Manager);
                }
            }
        }

        private RegistrationManager? Add(Type type, string name, int nameHash, RegistrationManager manager)
        {
            var hash = Contract.GetHashCode(type.GetHashCode(), nameHash);
            ref var bucket = ref Meta[((uint)hash) % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (ReferenceEquals(candidate.Contract.Type, type) &&
                    candidate.Contract.Name == name)
                {
                    var replacement   = candidate.Manager;
                    candidate.Manager = manager;
                    Revision += 1;

                    return replacement;
                }

                position = Meta[position].Next;
            }

            ref var @default = ref Data[GetDefault(type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(hash, type, name, manager, @default.Next);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            @default.Next = Index;
            Revision += 1;
            return null;
        }

        private int GetDefault(Type type)
        {
            var hash = type.GetHashCode();
            ref var bucket = ref Meta[((uint)hash) % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (ReferenceEquals(candidate.Contract.Type, type) && candidate.Contract.Name == null)
                {
                    // Found existing
                    return position;
                }

                position = Meta[position].Next;
            }

            // Add new registration
            Index++;
            Data[Index] = new Entry(hash, type);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            return Index;
        }

        #endregion


        #region Expand Storage

        private void Expand(int required)
        {
            Prime = Storage.Prime.IndexOf(required);
            Array.Resize(ref Data, Storage.Prime.Numbers[Prime++]);

            var meta = new Metadata[Storage.Prime.Numbers[Prime]];
            for (var current = START_INDEX; current <= Index; current++)
            {
                var bucket = ((uint)Data[current].Internal.Contract.HashCode) % meta.Length;
                meta[current].Next = meta[bucket].Position;
                meta[bucket].Position = current;
            }

            Meta = meta;
        }

        #endregion


        #region Nested Types

        [StructLayout(LayoutKind.Explicit)]
        private struct ContractUnion
        {
            [FieldOffset(0)] internal Contract Contract;
            [FieldOffset(0)] internal AsStruct AsStruct;

            public ContractUnion(string name)
            {
                Contract = default;
                AsStruct = new AsStruct(name);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AsStruct
        {
            // Do not change sequence

            public int HashCode;
            public Type Type;
            public string? Name;

            public AsStruct(string name)
            {
                Type = default!;
                Name = name;
                HashCode = 0;
            }
        }

        #endregion
    }
}
