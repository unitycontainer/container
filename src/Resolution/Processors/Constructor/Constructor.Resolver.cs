using System.Reflection;
using Unity.Builder;
using Unity.Resolution;
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
                    (ref BuilderContext context) => context.Error($"No accessible constructors on type {context.Type}");   
                
                return;
            }

            var ctorInfo = SelectConstructor(ref context, members);
            
            if (context.IsFaulted) return;

            ParameterResolver(ref context, ref ctorInfo);

            if (context.IsFaulted) return;

            context.Target = WithResolver(ref context, ref ctorInfo);
        }
        
        private FactoryBuilderStrategy WithResolver<TContext>(ref TContext context, ref InjectionInfoStruct<ConstructorInfo> info)
            where TContext : IBuildPlanContext<FactoryBuilderStrategy>
        {
            var constructor = info.MemberInfo;
            var parameters = (ResolverPipeline)info.DataValue.Value!;
            var resolver = context as FactoryBuilderStrategy;

            return resolver is null
                ? (ref BuilderContext context) =>
                {
                    try
                    {
                        context.Existing = constructor.Invoke((object[]?)parameters(ref context));
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
                }
                : (ref BuilderContext context) =>
                {
                    resolver(ref context);

                    if (context.Existing is not null) return;

                    try
                    {
                        context.Existing = constructor.Invoke((object[]?)parameters(ref context));
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

                    return;
                };
        }
    }
}
