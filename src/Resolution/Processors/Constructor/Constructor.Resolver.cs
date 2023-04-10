using System.Reflection;
using Unity.Builder;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class ConstructorProcessor
    {
        public override void BuildResolver<TContext>(ref TContext context)
        {
            var members = GetDeclaredMembers(context.TargetType);

            // Error if no constructors
            if (0 == members.Length)
            {
                context.Target =
                    (ref BuilderContext context) =>
                    {
                        if (null == context.Existing) context.Error($"No accessible constructors on type {context.Type}");
                    };   
                
                return;
            }

            var info = SelectConstructor(ref context, members);
            
            if (context.IsFaulted) return;

            base.BuildResolver(ref context, ref info);

            if (context.IsFaulted) return;

            if (context.Target is not null)
            {
                context.Target = info.InjectedValue.Type switch
                {
                    DataType.None     => GetResolver<TContext>(info.MemberInfo, EmptyParametersArray,  context.Target),
                    DataType.Value    => GetResolver<TContext>(info.MemberInfo, (object?[])info.InjectedValue.Value!, context.Target),
                    DataType.Pipeline => GetResolver<TContext>(info.MemberInfo, (ResolverPipeline)info.InjectedValue.Value!, context.Target),
                    _ => throw new NotImplementedException(),
                };
            }
            else
            {
                context.Target = info.InjectedValue.Type switch
                {
                    DataType.None     => GetResolver<TContext>(info.MemberInfo, EmptyParametersArray),
                    DataType.Value    => GetResolver<TContext>(info.MemberInfo, (object?[])info.InjectedValue.Value!),
                    DataType.Pipeline => GetResolver<TContext>(info.MemberInfo, (ResolverPipeline)info.InjectedValue.Value!),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private BuilderStrategyPipeline GetResolver<TContext>(ConstructorInfo constructor, object?[] parameters)
        {
            return (ref BuilderContext context) =>
            {
                if (context.IsFaulted || context.Target is not null) return;

                try
                {
                    context.Existing = constructor.Invoke(parameters);
                }
                catch (Exception ex) when (ex is ArgumentException ||
                                           ex is MemberAccessException)
                {
                    context.Error(ex.Message);
                }
                catch (Exception exception)
                {
                    context.Capture(exception);
                }
            };        
        }

        private BuilderStrategyPipeline GetResolver<TContext>(ConstructorInfo constructor, ResolverPipeline parameters)
        {
            return (ref BuilderContext context) =>
            {
                if (context.IsFaulted || context.Target is not null) return;

                try
                {
                    context.Existing = constructor.Invoke((object?[])parameters(ref context)!);
                }
                catch (Exception ex) when (ex is ArgumentException ||
                                           ex is MemberAccessException)
                {
                    context.Error(ex.Message);
                }
                catch (Exception exception)
                {
                    context.Capture(exception);
                }
            };
        }

        protected BuilderStrategyPipeline GetResolver<TContext>(ConstructorInfo constructor, object?[] parameters, BuilderStrategyPipeline pipeline)
        {
            return (ref BuilderContext context) =>
            {
                pipeline(ref context);

                if (context.IsFaulted || context.Target is not null) return;

                try
                {
                    context.Existing = constructor.Invoke(parameters);
                }
                catch (Exception ex) when (ex is ArgumentException ||
                                           ex is MemberAccessException)
                {
                    context.Error(ex.Message);
                }
                catch (Exception exception)
                {
                    context.Capture(exception);
                }
            };
        }

        protected BuilderStrategyPipeline GetResolver<TContext>(ConstructorInfo constructor, ResolverPipeline parameters, BuilderStrategyPipeline pipeline)
        {
            return (ref BuilderContext context) =>
            {
                pipeline(ref context);

                if (context.IsFaulted || context.Target is not null) return;

                try
                {
                    context.Existing = constructor.Invoke((object?[])parameters(ref context)!);
                }
                catch (Exception ex) when (ex is ArgumentException ||
                                           ex is MemberAccessException)
                {
                    context.Error(ex.Message);
                }
                catch (Exception exception)
                {
                    context.Capture(exception);
                }
            };
        }

    }
}
