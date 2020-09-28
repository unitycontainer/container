using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext 
    {
        
        public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class 
            => new PipelineAction<TAction>(ref this, action);

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

        public readonly ref PipelineContext Parent
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<PipelineContext>(_parent.ToPointer());
                }
            }
        }

        public PipelineContext DependencyContext(ref Contract contract, object? action)
            => new PipelineContext(ref this, ref contract, action);
    }
}
