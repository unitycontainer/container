using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData>
    {
        protected ResolverPipeline InjectedArrayResolver<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            Debug.Assert(info.InjectedValue.Value is not null);
            Debug.Assert(info.ContractType.IsArray);

//            var data = (object?[])info.InjectedValue.Value!;
//            var type = info.ContractType.GetElementType()!;

//            IList buffer;

//            try
//            {
//                buffer = Array.CreateInstance(type, data.Length);

//                for (var i = 0; i < data.Length; i++)
//                {
//                    var import = info.With(type!, data[i]);

//                    //BuildUpData<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref import);
//                    //BuildUpInfo(ref context, ref import);

//                    if (context.IsFaulted)
//                    {
//                        info.InjectedValue = default;
////                        return;
//                    }

//                    buffer[i] = import.InjectedValue.Value;
//                }
//            }
//            catch (Exception ex)
//            {
//                context.Error(ex.Message);
//                info.InjectedValue = default;
////                return;
//            }



            throw new NotImplementedException();
        }

        protected ResolverPipeline InjectedValueResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            object?  value = info.InjectedValue.Value;
            TMember  member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.FromInjectedValue(member, ref contract, value);
        }

        protected ResolverPipeline InjectedPipelineResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            ResolverPipeline pipeline = (ResolverPipeline?)info.InjectedValue.Value ?? throw new InvalidOperationException();
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.FromPipeline(member, ref contract, pipeline);
        }

        protected ResolverPipeline RequiredResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.Resolve(member, ref contract);
        }

        protected ResolverPipeline OptionalResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            throw new NotImplementedException();
            object? value = info.InjectedValue.Value;
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.FromInjectedValue(member, ref contract, value);
        }

        protected ResolverPipeline WithDefaultMethodResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            throw new NotImplementedException();
        }

        protected ResolverPipeline WithDefaultResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            throw new NotImplementedException();
        }
    }
}
