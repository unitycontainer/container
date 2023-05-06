using System.Runtime.InteropServices;

namespace Unity.Storage
{
    public partial class HashScope
    {
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
