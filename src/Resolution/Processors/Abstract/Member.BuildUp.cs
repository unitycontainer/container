using System.Collections;
using System.Diagnostics;
using Unity.Builder;
using Unity.Injection;
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

        protected void BuildUpInfo<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuilderContext
        {
            switch (info.InjectedValue.Type)
            {
                case DataType.Value:
                    info.InjectedValue[DataType.Value] = 
                        context.OverrideValue(info.MemberInfo, ref info.Contract, info.InjectedValue.Value);
                    break;

                case DataType.Pipeline:
                    info.InjectedValue[DataType.Value] =
                        context.OverridePipeline(info.MemberInfo, ref info.Contract, 
                                                 (ResolverPipeline)info.InjectedValue.Value!);
                    break;

                case DataType.Unknown:
                    throw new NotImplementedException();

                case DataType.Array:
                    BuildUpFromArray(ref context, ref info);
                    break;

                case DataType.None when DataType.None == info.DefaultValue.Type:
                    info.InjectedValue[DataType.Value] = context.Resolve(info.MemberInfo, ref info.Contract);
                    break;

                case DataType.None when DataType.Value == info.DefaultValue.Type:
                    info.InjectedValue[DataType.Value] = 
                        context.ResolveOptional(info.MemberInfo, ref info.Contract, info.DefaultValue.Value);
                    break;

                case DataType.None when DataType.Pipeline == info.DefaultValue.Type:
                    info.InjectedValue[DataType.Value] =
                        context.ResolveOptional(info.MemberInfo, ref info.Contract, (ResolverPipeline?)info.DefaultValue.Value);
                    break;

                default:
                    info.InjectedValue[DataType.Value] =
                        context.ResolveOptional(info.MemberInfo, ref info.Contract, (ResolverPipeline)GetDefaultValue);
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

                    EvaluateData(ref context, ref import);
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
    }
}
