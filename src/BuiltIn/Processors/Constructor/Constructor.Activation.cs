using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        protected void Activate(ref PipelineContext context, object?[] data)
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.Action!);

            var parameters = info.GetParameters();
            if (0 == parameters.Length)
            {
                context.Target = info.Invoke(EmptyParametersArray);
                return;
            }

            var arguments = new object?[parameters.Length];
            var parameter = new MemberDependency(ref context);

            for (var i = 0; i < parameters.Length; i++)
            {
                parameter.Info = parameters[i];
                arguments[i] = Activate(ref parameter, data[i]);

                if (context.IsFaulted) return;
            }

            context.Target = info.Invoke(arguments);
        }

        protected void Activate(ref PipelineContext context)
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.Action!);

            var parameters = info.GetParameters();
            if (0 == parameters.Length)
            {
                context.Target = info.Invoke(EmptyParametersArray);
                return;
            }

            var arguments = new object?[parameters.Length];
            var parameter = new MemberDependency(ref context);

            for (var i = 0; i < parameters.Length; i++)
            {
                parameter.Info = parameters[i];
                arguments[i] = Activate(ref parameter);
                
                if (context.IsFaulted) return;
            }

            context.Target = info.Invoke(arguments);
        }
    }
}
