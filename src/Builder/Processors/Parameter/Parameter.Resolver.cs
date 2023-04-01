using System.Runtime.CompilerServices;
using Unity.Resolution;
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
                info.DataValue[DataType.Pipeline] = (ResolveDelegate<TContext>)
                    ((ref TContext c) => ParameterProcessor<TContext, TMemberInfo>.EmptyParametersArray);
                
                return;
            }
            
            //
        }
    }
}
