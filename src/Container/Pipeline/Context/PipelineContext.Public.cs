using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct PipelineContext 
    {
        
        public PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class 
            => new PipelineAction<TAction>(ref this) { Target = action };


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

    }
}
