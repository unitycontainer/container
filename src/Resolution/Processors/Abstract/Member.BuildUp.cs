using System.Collections;
using System.Diagnostics;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData>
    {
        public override void BuildUp<TContext>(ref TContext context)
        {
            Debug.Assert(null != context.Existing, "Target should never be null");
            var members = GetDeclaredMembers(context.Type);

            if (0 == members.Length) return;

            try
            {
                var enumerator = SelectMembers(ref context, members);
                while (enumerator.MoveNext())
                {
                    ref var current = ref enumerator.Current;

                    var @override = context.GetResolverOverride(current.MemberInfo, ref current.Contract);
                    if (@override is not null) current.Data = @override.Resolve(ref context);

                    EvaluateInfo(ref context, ref current);
                    BuildUpInfo(ref context, ref current);
                    BuildUpMember(ref context, ref current);
                }
            }
            catch (ArgumentException ex)
            {
                context.Error(ex.Message);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }
        }

        protected virtual void BuildUpInfo<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuilderContext
        {
            switch (info.InjectedValue.Type)
            {
                case DataType.None:
                    context.Resolve(ref info);
                    break;

                case DataType.Array:
                    BuildUpFromArray(ref context, ref info);
                    break;

                default:
                    break;
            };
        }

        protected virtual void BuildUpMember<TContext>(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
            where TContext : IBuilderContext => throw new NotImplementedException();

        protected void BuildUpFromArray<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuilderContext
        {
            Debug.Assert(info.InjectedValue.Value is not null);
            Debug.Assert(info.ContractType.IsArray);

            var data = (object?[])info.InjectedValue.Value!;
            var type = info.ContractType.GetElementType()!;

            IList buffer;

            try
            {
                buffer = Array.CreateInstance(type, data.Length);

                for (var i = 0; i < data.Length; i++)
                {
                    var import = info.With(type!, data[i]);

                    EvaluateInfo(ref context, ref import);
                    BuildUpInfo(ref context, ref import);

                    if (context.IsFaulted)
                    {
                        info.InjectedValue = default;
                        return;
                    }

                    buffer[i] = import.InjectedValue.Value;
                }
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
                info.InjectedValue = default;
                return;
            }

            info.InjectedValue[DataType.Value] = buffer;
        }

        private static object? BuildUpFromPipeline<TContext, TMember, TInjectionInfo>(ref TContext context, ref TInjectionInfo info, ResolverPipeline @delegate)
            where TContext : IBuildPlanContext
            where TInjectionInfo : IInjectionInfo<TMember>
        {
            var request = new BuilderContext.RequestInfo();
            var contract = new Contract(info.ContractType, info.ContractName);
            var builderContext = request.Context(context.Container, ref contract);

            try
            {
                return @delegate(ref builderContext);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }

            return UnityContainer.NoValue;
        }
    }
}
