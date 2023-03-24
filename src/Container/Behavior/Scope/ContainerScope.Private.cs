using System;
using System.Runtime.InteropServices;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Add named/default

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

                if (type is null) return;

                Add(type, descriptor.Manager);
            }
            else
            {
                // Anonymous contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    // TODO: Proper error handling
                    if (type is null) continue;

                    Add(type, descriptor.Manager);
                }
            }
        }

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

                if (type is null) return;

                union.AsStruct.Type = type;
                union.AsStruct.HashCode = Contract.GetHashCode(type.GetHashCode(), nameHash);

                Add(in union.Contract, descriptor.Manager);
            }
            else
            {
                // Register contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    if (type is null) continue;

                    union.AsStruct.Type = type;
                    union.AsStruct.HashCode = Contract.GetHashCode(type.GetHashCode(), nameHash);
                    
                    // TODO: Add dealing with replacement
                    Add(type, name, nameHash, descriptor.Manager);
                }
            }
        }

        #endregion


        #region Add

        private RegistrationManager? Add(Type type, RegistrationManager manager)
        {
            var hash = type.GetHashCode();
            ref var bucket = ref Meta[((uint)hash) % Meta.Length];
            var position = bucket.Position;
            int pointer = 0;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Internal.Contract.Type, type) &&
                    candidate.Internal.Contract.Name == null)
                {
                    if (candidate.Internal.Manager is null)
                    {
                        var old = candidate.Internal.Manager;
                        candidate.Internal.Manager = manager;
                        Revision += 1;
                        return old;
                    }

                    // move pointer no next into default
                    pointer = candidate.Next;
                    candidate.Next = 0;

                    goto register;
                }

                position = Meta[position].Location;
            }

            // Add new registration
            register: Index++;
            Data[Index] = new Entry(hash, type, manager, pointer);
            Meta[Index].Location = bucket.Position;
            bucket.Position = Index;
            Revision += 1;
            return null;
        }


        private RegistrationManager? Add(in Contract contract, RegistrationManager manager)
        {
            ref var bucket = ref Meta[((uint)contract.HashCode) % Meta.Length];
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

                position = Meta[position].Location;
            }

            ref var @default = ref Data[GetDefault(contract.Type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(in contract, manager, @default.Next);
            Meta[Index].Location = bucket.Position;
            bucket.Position = Index;
            @default.Next = Index;
            Revision += 1;
            return null;
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

                position = Meta[position].Location;
            }

            ref var @default = ref Data[GetDefault(type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(hash, type, name, manager, @default.Next);
            Meta[Index].Location = bucket.Position;
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

                position = Meta[position].Location;
            }

            // Add new registration
            Index++;
            Data[Index] = new Entry(hash, type);
            Meta[Index].Location = bucket.Position;
            bucket.Position = Index;
            return Index;
        }

        #endregion


        #region Expand Storage

        private void Expand(int required)
        {
            Prime = Storage.Prime.NextUp(required);
            Array.Resize(ref Data, Storage.Prime.Numbers[Prime++]);

            var meta = new Metadata[Storage.Prime.Numbers[Prime]];
            for (var current = START_INDEX; current <= Index; current++)
            {
                var bucket = ((uint)Data[current].Internal.Contract.HashCode) % meta.Length;
                meta[current].Location = meta[bucket].Position;
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
