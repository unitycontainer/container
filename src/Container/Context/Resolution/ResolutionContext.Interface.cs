using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
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
    }
}
