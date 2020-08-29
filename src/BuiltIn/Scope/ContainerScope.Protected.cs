using System;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        /// <summary>
        /// Add a contract to the scope
        /// </summary>
        /// <remarks>
        /// This method adds a new or updates existing <see cref="Contract"/>. The method is used 
        /// by registration code and updates <see cref="Version"/> on each execution with both: new and 
        /// existing registrations.
        /// </remarks>
        /// <param name="contract"><see cref="Contract"/> to add to the scope</param>
        /// <param name="manager"><see cref="RegistrationManager"/> to add to <see cref="Contract"/></param>
        /// <returns>Position where <see cref="Contract"/> is inserted or 0 if <see cref="Contract"/> 
        /// already existed</returns>
        protected virtual int Add(in Contract contract, RegistrationManager manager)
        {
            var hash = (uint)contract.HashCode;
            ref var bucket = ref ContractsMeta[hash % ContractsMeta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref ContractsData[position];
                if (ReferenceEquals(candidate._contract.Type, contract.Type) && 
                    ReferenceEquals(candidate._contract.Name, contract.Name))
                {
                    // Found existing
                    candidate = new ContainerRegistration(in contract, manager);
                    _version += 1;
                    return 0;
                }

                position = ContractsMeta[position].Next;
            }

            // Add new registration
            ContractsCount++;
            ContractsData[ContractsCount] = new ContainerRegistration(in contract, manager);
            ContractsMeta[ContractsCount].Next = bucket.Position;
            bucket.Position = ContractsCount;
            _version += 1;

            return ContractsCount;
        }

        
        #region Expanding Contracts

        /// <summary>
        /// Expand contracts storage one <see cref="Prime.Numbers"/> size up 
        /// </summary>
        protected virtual void Expand()
        {
            Array.Resize(ref ContractsData, Prime.Numbers[ContractsPrime++]);

            var meta = new Metadata[Prime.Numbers[ContractsPrime]];
            for (var current = START_INDEX; current <= ContractsCount; current++)
            {
                var bucket = (uint)ContractsData[current]._contract.HashCode % meta.Length;
                meta[current].Next = meta[bucket].Position;
                meta[bucket].Position = current;
            }

            ContractsMeta = meta;
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
        protected virtual void Expand(int required)
        {
            ContractsPrime = Prime.IndexOf(required);
            Array.Resize(ref ContractsData, Prime.Numbers[ContractsPrime++]);

            var meta = new Metadata[Prime.Numbers[ContractsPrime]];
            for (var current = START_INDEX; current <= ContractsCount; current++)
            {
                var bucket = (uint)ContractsData[current]._contract.HashCode % meta.Length;
                meta[current].Next = meta[bucket].Position;
                meta[bucket].Position = current;
            }

            ContractsMeta = meta;
        }

        #endregion
    }
}
