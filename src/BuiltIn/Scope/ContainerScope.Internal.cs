using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Add

        private void AddAnonymous(in RegistrationDescriptor descriptor)
        {
            ContractUnion union = default;

            if (0 == descriptor.RegisterAs.Length)
            {
                var type = descriptor.Manager.Category switch
                {
                    RegistrationCategory.Type     => descriptor.Manager.Type,

                    // TODO: Proper error handling
                    RegistrationCategory.Instance when null == descriptor.Manager.Data
                        => throw new ArgumentException($"Registration Manager {descriptor.Manager} is invalid", "manager"),
                    
                    RegistrationCategory.Instance => descriptor.Manager.Instance!.GetType(),

                    _ => throw new ArgumentException($"Registration Manager {descriptor.Manager} is invalid", "manager"),
                };
                
                if (null == type) return;
                
                union.AsStruct.Type = type;
                union.AsStruct.HashCode = type.GetHashCode();

                Set(in union.Contract, descriptor.Manager);
            }
            else
            { 
                // Anonymous contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    if (null == type) continue;

                    union.AsStruct.Type = type;
                    union.AsStruct.HashCode = type.GetHashCode();

                    Set(in union.Contract, descriptor.Manager);
                }
            }
        }

        private void AddWithName(in RegistrationDescriptor descriptor)
        {
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
                union.AsStruct.HashCode = type.GetHashCode();

                SetWithName(in union.Contract, descriptor.Manager);
            }
            else
            {
                // Register contracts
                foreach (var type in descriptor.RegisterAs)
                {
                    if (null == type) continue;

                    union.AsStruct.Type = type;
                    union.AsStruct.HashCode = Contract.GetHashCode(type, descriptor.Name);

                    SetWithName(in union.Contract, descriptor.Manager);
                }
            }
        }


        /// <summary>
        /// Set a <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// This method follows original Unity specification and replaces registration 
        /// with new one if contracts match.
        /// </remarks>
        /// <param name="contract"><see cref="Contract"/> to add to the scope</param>
        /// <param name="manager"><see cref="RegistrationManager"/> to add to <see cref="Contract"/></param>
        private void Set(in Contract contract, RegistrationManager manager)
        {
            var hash = (uint)contract.HashCode;
            ref var bucket = ref Meta[hash % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Registration;
                if (!ReferenceEquals(candidate._contract.Type, contract.Type) || candidate._contract.Name != null)
                    position = Meta[position].Next;
            }

            // Add new registration
            Index++;
            Data[Index] = new Entry(in contract, manager);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            Revision += 1;
        }


        private void SetWithName(in Contract contract, RegistrationManager manager)
        {
            var hash = (uint)contract.HashCode;
            ref var bucket = ref Meta[hash % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Registration;
                if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                    candidate._contract.Name == contract.Name)
                {
                    // Found existing
                    candidate = new ContainerRegistration(in contract, manager);
                    Revision += 1;
                    return;
                }

                position = Meta[position].Next;
            }

            var def = GetBase(contract.Type);

            // Add new registration
            Index++;
            Data[Index] = new Entry(in contract, manager);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            Revision += 1;
        }

        #endregion


        private ref Entry GetBase(Type type)
        {
            var hash = (uint)type.GetHashCode();
            ref var bucket = ref Meta[hash % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Registration;
                if (ReferenceEquals(candidate._contract.Type, type) && candidate._contract.Name == null)
                {
                    // Found existing
                    return ref Data[position];
                }

                position = Meta[position].Next;
            }

            // Add new registration
            Index++;
            Data[Index] = new Entry(type);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            return ref Data[Index];
        }


        #region Expanding Contracts

        /// <summary>
        /// Expand contracts storage one <see cref="Prime.Numbers"/> size up 
        /// </summary>
        private void Expand()
        {
            Array.Resize(ref Data, Storage.Prime.Numbers[Prime++]);

            var meta = new Metadata[Storage.Prime.Numbers[Prime]];
            for (var current = START_INDEX; current <= Index; current++)
            {
                var bucket = (uint)Data[current].Registration._contract.HashCode % meta.Length;
                meta[current].Next = meta[bucket].Position;
                meta[bucket].Position = current;
            }

            Meta = meta;
        }

        /// <summary>
        /// Expand contracts storage to accommodate requested amount
        /// </summary>
        /// <remarks>
        /// This method resizes storage to hold at least <paramref name="required"/> number
        /// of contracts. It checks <see cref="Prime.Numbers"/> collection and selects first
        /// prime number that is equal or bigger than the <paramref name="required"/>.
        /// </remarks>
        /// <param name="required">Total required size</param>
        private void Expand(int required)
        {
            Prime = Storage.Prime.IndexOf(required);
            Array.Resize(ref Data, Storage.Prime.Numbers[Prime++]);

            var meta = new Metadata[Storage.Prime.Numbers[Prime]];
            for (var current = START_INDEX; current <= Index; current++)
            {
                var bucket = (uint)Data[current].Registration._contract.HashCode % meta.Length;
                meta[current].Next = meta[bucket].Position;
                meta[bucket].Position = current;
            }

            Meta = meta;
        }

        #endregion


        #region Contract Proxy

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
