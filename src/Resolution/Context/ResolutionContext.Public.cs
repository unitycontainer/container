using System;
using System.Runtime.CompilerServices;

namespace Unity.Resolution
{
    public partial struct ResolutionContext : IResolveContext
    {
        #region IResolveContext

        public readonly Type Type
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Type;
                }
            }
        }

        public readonly string? Name
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Name;
                }
            }
        }

        public object? Resolve(Type type, string? name)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Contract

        public readonly ref Contract Contract
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<Contract>(_contract.ToPointer());
                }
            }
        }

        #endregion


        #region RequestRoot

        public readonly ResolverOverride[] Overrides
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<RequestInfo>(_request.ToPointer()).Overrides;
                }
            }
        }

        #endregion
    }
}
