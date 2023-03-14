using System.Reflection;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        public override object? Analyse<TContext>(ref TContext context)
        {
            // Load attributes and injection data
            var analysis = base.Analyse(ref context);
            if (analysis is null) return analysis;

            var descriptors = (MemberDescriptor<TContext, TMemberInfo>[])analysis;
            for (var i = 0; i < descriptors.Length; i++)
            {
                ref var descriptor = ref descriptors[i];
                descriptor.ValueData = descriptor.ValueData.Type == ImportType.None ||
                                       descriptor.ValueData.Value is not object?[] array || 0 == array.Length
                    ? Analyse(ref context, ref descriptor)
                    : Analyse(ref context, ref descriptor, (object?[])descriptor.ValueData.Value!);
            }

            return analysis;
        }

        protected override void Analyse<TContext>(ref TContext context, ref MemberDescriptor<TContext, TMemberInfo> descriptor, InjectionMember<TMemberInfo, object[]> member) 
            => member.ProvideImport<TContext, MemberDescriptor<TContext, TMemberInfo>>(ref descriptor);


        private ImportData Analyse<TContext>(ref TContext context, ref MemberDescriptor<TContext, TMemberInfo> member) 
            where TContext : IBuilderContext
        {
            var parameters = member.MemberInfo.GetParameters();
            if (0 == parameters.Length) return default;

            var descriptors = new MemberDescriptor<TContext, ParameterInfo>[parameters.Length];

            for (var i = 0; i < descriptors.Length; i++)
            {
                // Load descriptor from metadata
                ref var descriptor = ref descriptors[i];

                descriptor.MemberInfo = parameters[i];

                ParameterProvider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref descriptor);
            }

            return new ImportData(descriptors, ImportType.Value);
        }

        private ImportData Analyse<TContext>(ref TContext context, ref MemberDescriptor<TContext, TMemberInfo> member, object?[] data)
            where TContext : IBuilderContext
        {
            var parameters = member.MemberInfo.GetParameters();
            if (0 == parameters.Length) return default;

            var descriptors = new MemberDescriptor<TContext, ParameterInfo>[parameters.Length];

            for (var i = 0; i < descriptors.Length; i++)
            {
                // Load descriptor from metadata
                ref var descriptor = ref descriptors[i];

                descriptor.MemberInfo = parameters[i];

                ParameterProvider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref descriptor);
                descriptor.Dynamic = data[i];

                while (ImportType.Dynamic == descriptor.ValueData.Type)
                    Analyse(ref context, ref descriptor);
            }


            return new ImportData(descriptors, ImportType.Value);
        }
    }
}
