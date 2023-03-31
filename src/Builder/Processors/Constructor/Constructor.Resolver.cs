using System.Reflection;
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
                context.Existing = ErrorResolver($"No accessible constructors on type {context.Type}");                
                return;
            }

            var ctorInfo = SelectConstructor(ref context, members);
            
            if (context.IsFaulted)
            {
                var error = context.ErrorInfo.Message ?? $"No accessible constructors on type {context.Type}";                
                context.Existing = ErrorResolver(error);
                return;
            }

            ParameterResolver(ref context, ref ctorInfo);

            if (context.IsFaulted)
            {
                var error = context.ErrorInfo.Message ?? $"No accessible constructors on type {context.Type}";
                context.Existing = ErrorResolver(error);
                return;
            }

            var parameters = (ResolveDelegate<TContext>)ctorInfo.DataValue.Value;
            context.Existing = (ref TContext context) => 
            {
                try
                {
                    context.Instance = ctorInfo.MemberInfo.Invoke((object[])parameters(ref context));
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

                return context.Existing;
            };
        }

        private ResolveDelegate<TContext> ErrorResolver(string error)
            => (ref TContext context) => context.Error(error);
    }
}
