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

                AddDefault(type, descriptor.Manager);
            }
            else
            {
                // Anonymous contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    // TODO: Proper error handling
                    if (null == type) continue;

                    AddDefault(type, descriptor.Manager);
                }
            }
        }

        private void AddDefault(Type type, RegistrationManager manager)
        {
            var hash = type.GetHashCode();
            ref var bucket = ref Meta[((uint)hash) % Meta.Length];
            var position = bucket.Position;
            long pointer = 0;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Internal.Contract.Type, type) &&
                    candidate.Internal.Contract.Name == null)
                {
                    if (null == candidate.Internal.Manager)
                    {
                        candidate.Internal.Manager = manager;
                        Revision += 1;
                        return;
                    }

                    // move pointer no next into default
                    pointer = candidate.Next;
                    candidate.Next = 0;

                    goto RegisterNew;
                }

                position = Meta[position].Next;
            }

            // Add new registration
            RegisterNew: Index++;
            Data[Index] = new Entry(hash, type, manager, pointer);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            Revision += 1;
        }

        #endregion


        #region Add Contract

        private void AddContract(in RegistrationDescriptor descriptor)
        {
            var name = descriptor.Name!;
            var hash = name.GetHashCode();
            
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
                union.AsStruct.HashCode = Contract.GetHashCode(type.GetHashCode(), hash);

                AddContract(in union.Contract, descriptor.Manager);
            }
            else
            {
                // Register contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    if (null == type) continue;

                    union.AsStruct.Type = type;
                    union.AsStruct.HashCode = Contract.GetHashCode(type.GetHashCode(), hash);
                    
                    // TODO: Add dealing with replacement
                    AddContract(type, hash, name, descriptor.Manager);
                }
            }
        }


        private RegistrationManager? AddContract(in Contract contract, RegistrationManager manager)
        {
            var hash = (uint)contract.HashCode;
            ref var bucket = ref Meta[hash % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                    candidate.Contract.Name == contract.Name)
                {
                    var replacement = candidate.Manager;
                    candidate.Manager = manager;
                    Revision += 1;

                    return replacement;
                }

                position = Meta[position].Next;
            }

            ref var @default = ref Data[GetDefault(contract.Type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(in contract, manager, @default.Next);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            @default.Next = Index;
            Revision += 1;
            return null;
        }


        private RegistrationManager? AddContract(Type type, int nameHash, string name, RegistrationManager manager)
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
