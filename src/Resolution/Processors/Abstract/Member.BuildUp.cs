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

                    var @override = context.GetOverride(current.MemberInfo, ref current.Contract);
                    if (@override is not null) current.Data = @override.Resolve(ref context);

                    BuildUpData<TContext, TMemberInfo, InjectionInfoStruct<TMemberInfo>>(ref context, ref current);
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

        protected virtual void BuildUpData<TContext, TMember, TInjectionInfo>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext       : IBuilderContext
            where TInjectionInfo : IInjectionInfo<TMember>
        {
            do
            {
                switch (info.InjectedValue.Value)
                {
                    case IInjectionInfoProvider<TMember> provider:
                        info.InjectedValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IInjectionInfoProvider provider:
                        info.InjectedValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.InjectedValue[DataType.Unknown] = MemberProcessor<TMemberInfo, TData>
                            .BuildUpFromPipeline<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref info, iResolve.Resolve);
                        break;

                    case ResolverPipeline resolver:
                        info.InjectedValue[DataType.Unknown] = MemberProcessor<TMemberInfo, TData>
                            .BuildUpFromPipeline<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref info, resolver);
                        break;

                    case IResolverFactory<TMember> factory:
                        info.InjectedValue[DataType.Unknown] = MemberProcessor<TMemberInfo, TData>
                            .BuildUpFromPipeline<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref info,
                            factory.GetResolver<BuilderContext>(info.MemberInfo));
                        break;

                    case IResolverFactory<Type> factory:
                        info.InjectedValue[DataType.Unknown] = MemberProcessor<TMemberInfo, TData>
                            .BuildUpFromPipeline<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref info, factory.GetResolver<BuilderContext>(info.MemberType));
                        break;

                    case ResolverFactory<BuilderContext> factory:
                        info.InjectedValue[DataType.Unknown] = MemberProcessor<TMemberInfo, TData>
                            .BuildUpFromPipeline<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref info, factory(info.ContractType));
                        break;

                    case Type target when typeof(Type) != info.MemberType:
                        info.ContractType = target;
                        info.ContractName = null;
                        info.AllowDefault = false;
                        info.DefaultValue = default;
                        info.InjectedValue = default;
                        return;

                    case UnityContainer.InvalidValue _:
                        info.DefaultValue = default;
                        return;

                    case null when DataType.None == info.InjectedValue.Type:
                    case Array when DataType.Array == info.InjectedValue.Type:
                        return;

                    default:
                        info.InjectedValue.Type = DataType.Value;
                        return;
                }
            }
            while (!context.IsFaulted && DataType.Unknown == info.InjectedValue.Type);
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

                    BuildUpData<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref import);
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
