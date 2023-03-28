using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TContext, TMemberInfo>
    {
        public override object? Analyze(ref TContext context)
        {
            // Load attributes and injection data
            var analysis = base.Analyze(ref context);
            if (analysis is null) return analysis;

            var descriptors = (MemberDescriptor<TMemberInfo>[])analysis;
            for (var i = 0; i < descriptors.Length; i++)
            {
                ref var descriptor = ref descriptors[i];
                descriptor.ValueData = descriptor.ValueData.Type == ImportType.None ||
                                       descriptor.ValueData.Value is not object?[] array || 0 == array.Length
                    ? Analyze(ref context, ref descriptor)
                    : Analyze(ref context, ref descriptor, (object?[])descriptor.ValueData.Value!);
            }

            return analysis;
        }

        protected override void Analyze(ref TContext context, ref MemberDescriptor<TMemberInfo> descriptor, InjectionMember<TMemberInfo, object[]> member) 
            => member.ProvideInfo(ref descriptor);


        private ImportData Analyze(ref TContext context, ref MemberDescriptor<TMemberInfo> member) 
        {
            var parameters = member.MemberInfo.GetParameters();
            if (0 == parameters.Length) return default;

            var descriptors = new MemberDescriptor<ParameterInfo>[parameters.Length];

            for (var i = 0; i < descriptors.Length; i++)
            {
                // Load descriptor from metadata
                ref var descriptor = ref descriptors[i];

                descriptor.MemberInfo = parameters[i];

                ParameterProvider.ProvideInfo(ref descriptor);
            }

            return new ImportData(descriptors, ImportType.Value);
        }

        private ImportData Analyze(ref TContext context, ref MemberDescriptor<TMemberInfo> member, object?[] data)
        {
            var parameters = member.MemberInfo.GetParameters();
            if (0 == parameters.Length) return default;

            var descriptors = new MemberDescriptor<ParameterInfo>[parameters.Length];

            for (var i = 0; i < descriptors.Length; i++)
            {
                // Load descriptor from metadata
                ref var descriptor = ref descriptors[i];

                descriptor.MemberInfo = parameters[i];

                ParameterProvider.ProvideInfo(ref descriptor);
                descriptor.Data = data[i];

                while (ImportType.Unknown == descriptor.ValueData.Type)
                    Analyze(ref context, ref descriptor);
            }


            return new ImportData(descriptors, ImportType.Value);
        }
    }
}
