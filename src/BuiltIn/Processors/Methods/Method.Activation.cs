using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        protected override void Activate(ref PipelineContext context, object?[] data)
        {
            MethodInfo info = Unsafe.As<MethodInfo>(context.Action!);

            var parameters = info.GetParameters();
            if (0 == parameters.Length)
            {
                context.Target = info.Invoke(context.Target, EmptyParametersArray);
                return;
            }

            var arguments = new object?[parameters.Length];
            var parameter = new DependencyContext<ParameterInfo>(ref context);

            for (var i = 0; i < parameters.Length; i++)
            {
                parameter.Info = parameters[i];
                arguments[i] = Activate(ref parameter, data[i]);

                if (context.IsFaulted) return;
            }

            context.Target = info.Invoke(context.Target, arguments);
        }

        protected void Activate(ref PipelineContext context)
        {
            MethodInfo info = Unsafe.As<MethodInfo>(context.Action!);

            var parameters = info.GetParameters();
            if (0 == parameters.Length)
            {
                context.Target = info.Invoke(context.Target, EmptyParametersArray);
                return;
            }

            var arguments = new object?[parameters.Length];
            var parameter = new DependencyContext<ParameterInfo>(ref context);

            for (var i = 0; i < parameters.Length; i++)
            {
                parameter.Info = parameters[i];
                arguments[i] = Activate(ref parameter);

                if (context.IsFaulted) return;
            }

            context.Target = info.Invoke(context.Target, arguments);
        }
    }
}
