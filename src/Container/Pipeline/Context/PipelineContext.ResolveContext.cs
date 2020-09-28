using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext : IResolveContext
    {
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

        public string? Name
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
            var contract = new Contract(type, name);
            return Container.ResolveDependency(ref contract, ref this);
        }
    }
}
