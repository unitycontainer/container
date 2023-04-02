using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public virtual void ParameterResolver<TContext>(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();

            if (0 == parameters.Length)
            {
                info.DataValue[DataType.Pipeline] = (ResolverPipeline)
                    ((ref BuilderContext c) => ParameterProcessor<TMemberInfo>.EmptyParametersArray);
                
                return;
            }
            
            //
        }
    }
}
