using System.Reflection;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        public override void BuildUp(ref TContext context)
        {
            // Do nothing if building up
            if (null != context.Existing) return;

            Type type = context.Type;
            var members = GetDeclaredMembers(type);

            // Error if no constructors
            if (0 == members.Length)
            {
                context.Error($"No accessible constructors on type {type}");
                return;
            }

            try
            {
                // Select injected or annotated constructor, if available
                var enumerator = SelectMembers(ref context, members);
                while (enumerator.MoveNext())
                {
                    ref var current = ref enumerator.Current;

                    BuildUp(ref context, ref current);
                    context.Instance = current.MemberInfo.Invoke((object[]?)current.DataValue.Value);
                    return;
                }

                // Only one constructor, nothing to select
                if (1 == members.Length)
                {
                    var single  = members[0];
                    var @struct = new InjectionInfoStruct<ConstructorInfo>(single, single.DeclaringType!);

                    BuildUp(ref context, ref @struct);
                    context.Instance = single.Invoke((object[]?)@struct.DataValue.Value);
                    return;
                }

                // Select using algorithm
                ConstructorInfo? selected = SelectAlgorithmically(ref context, members);
                if (null != selected)
                {
                    var @struct = new InjectionInfoStruct<ConstructorInfo>(selected, selected.DeclaringType!);

                    BuildUp(ref context, ref @struct);
                    context.Instance = selected.Invoke((object[]?)@struct.DataValue.Value);
                    return;
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

            context.Error($"No accessible constructors on type {type}");
        }


        private void PreBuildUp(ref TContext context, ConstructorInfo info)
        {
            var parameters = info.GetParameters();
            var arguments  = 0 == parameters.Length
                ? EmptyParametersArray
                : BuildUp(ref context, parameters);
            
            context.Instance = info.Invoke(arguments);
        }

    }
}
