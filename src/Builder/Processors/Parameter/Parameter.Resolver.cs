using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public virtual void ParameterResolver<TContext>(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
            where TContext : IBuilderContext
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();

            if (0 == parameters.Length)
            {
                info.DataValue[DataType.Pipeline] = (ResolveDelegate<TContext>)
                    ((ref TContext c) => ParameterProcessor<TMemberInfo>.EmptyParametersArray);
                
                return;
            }
            
            //
        }
    }
}
