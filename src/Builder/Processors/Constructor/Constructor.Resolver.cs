using System.Reflection;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        public override void BuildResolver(ref TContext context)
        {
            var members = GetDeclaredMembers(context.Type);

            // Error if no constructors
            if (0 == members.Length)
            {
                context.Existing = (BuilderStrategyDelegate<TContext>)
                    ((ref TContext context) => context.Error($"No accessible constructors on type {context.Type}"));   
                
                return;
            }

            var ctorInfo = SelectConstructor(ref context, members);
            
            if (context.IsFaulted) return;

            ParameterResolver(ref context, ref ctorInfo);

            if (context.IsFaulted) return;

            context.Existing = WithResolver(ref context, ref ctorInfo);
        }
        
        private BuilderStrategyDelegate<TContext> WithResolver(ref TContext context, ref InjectionInfoStruct<ConstructorInfo> info)
        {
            var constructor = info.MemberInfo;
            var parameters = (ResolveDelegate<TContext>)info.DataValue.Value!;
            var resolver = context as BuilderStrategyDelegate<TContext>;

            return resolver is null
                ? (ref TContext context) =>
                {
                    try
                    {
                        context.Instance = constructor.Invoke((object[]?)parameters(ref context));
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
                : (ref TContext context) =>
                {
                    resolver(ref context);

                    if (context.Existing is not null) return;

                    try
                    {
                        context.Instance = constructor.Invoke((object[]?)parameters(ref context));
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
