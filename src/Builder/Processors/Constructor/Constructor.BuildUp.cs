using System.Reflection;
using Unity.Builder;
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

            ///////////////////////////////////////////////////////////////////
            // Error if no constructors
            if (0 == members.Length)
            {
                context.Error($"No accessible constructors on type {type}");
                return;
            }

            try
            {
                ///////////////////////////////////////////////////////////////////
                // Inject the constructor, if available
                var constructors = context.Registration?.Constructors;
                if (constructors is not null && 0 < constructors.Length)
                {
                    int index;
                    Span<int> set = stackalloc int[members.Length];
                    var ctor = constructors[0];

                    if (-1 == (index = SelectMember(ctor, members, ref set)))
                    {
                        context.Error($"Injected constructor '{ctor}' doesn't match any accessible constructors on type {type}");
                        return;
                    }

                    var member = members[index];
                    var descriptor = new InjectionInfoStruct<ConstructorInfo>(member, member.DeclaringType!);
                    ctor.ProvideInfo(ref descriptor);

                    BuildUp(ref context, ref descriptor);

                    context.Instance = descriptor.MemberInfo.Invoke((object[]?)descriptor.DataValue.Value);
                    return;
                }


                ///////////////////////////////////////////////////////////////////
                // Only one constructor, nothing to select
                if (1 == members.Length)
                {
                    PreBuildUp(ref context, members[0]);
                    return;
                }


                ///////////////////////////////////////////////////////////////////
                // Check for annotated constructor
                foreach (var member in members)
                {
                    var descriptor = new InjectionInfoStruct<ConstructorInfo>(member, member.DeclaringType!);

                    ProvideInjectionInfo(ref descriptor);

                    if (!descriptor.IsImport) continue;

                    BuildUp(ref context, ref descriptor);

                    context.Instance = member.Invoke((object[]?)descriptor.DataValue.Value);
                    return;
                }


                ///////////////////////////////////////////////////////////////////
                // Select using algorithm
                ConstructorInfo? info = SelectAlgorithmically(ref context, members);
                if (null != info)
                {
                    PreBuildUp(ref context, info);
                    return;
                }
            }
            catch (Exception ex)    // Catch errors from custom providers
            {
                context.Capture(ex);
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
