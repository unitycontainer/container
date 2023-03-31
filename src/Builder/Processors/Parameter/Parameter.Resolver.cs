using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TContext, TMemberInfo>
    {
        public virtual void ParameterResolver(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();

            if (0 == parameters.Length)
            {
                info.DataValue[DataType.Pipeline] = (ref TContext context) =>
                {
                    context.Existing = ParameterProcessor<TContext, TMemberInfo>.EmptyParametersArray;
                    return context.Existing;
                };

                return;
            }
            
            //
        }
    }
}
