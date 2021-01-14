using System;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        private static object? _empty;

        protected override void InjectImport<TContext>(ref ImportDescriptor<TMemberInfo> descroptor, InjectionMember<TMemberInfo, object[]> member)
        {
            var parameters = descroptor.MemberInfo.GetParameters();
            if (0 == parameters.Length)
            {
                descroptor.Value = _empty ??= new object[0];
                return;
            }

            descroptor.Dynamic = member.Data is null
                ? AnalyseParameters<TContext>(parameters)
                : AnalyseParameters<TContext>(parameters, member.Data);
            
        }

        protected ImportDescriptor<ParameterInfo>[] AnalyseParameters<TContext>(ParameterInfo[] parameters)
                    where TContext : IBuilderContext
        {
            var imports = new ImportDescriptor<ParameterInfo>[parameters.Length];

            // Load descriptor from metadata
            for (var i = 0; i < imports.Length; i++)
            {
                ref var descriptor = ref imports[i];

                descriptor.MemberInfo = parameters[i];
                DescribeParameter(ref descriptor);
            }

            return imports;
        }

        protected ImportDescriptor<ParameterInfo>[] AnalyseParameters<TContext>(ParameterInfo[] parameters, object[] data)
                    where TContext : IBuilderContext
        {
            var descriptors = AnalyseParameters<TContext>(parameters);

            // Load descriptor from metadata
            for (var i = 0; i < descriptors.Length; i++)
            {
                ref var descriptor = ref descriptors[i];

                descriptor.Dynamic = data[i];
                while (ImportType.Dynamic == descriptor.ValueData.Type)
                    Translate<ParameterInfo, ImportDescriptor<ParameterInfo>, TContext>(ref descriptor, descriptor.ValueData.Value);
            }

            return descriptors;
        }



        protected virtual void Translate<TMember, TDescriptor, TContext>(ref TDescriptor descriptor, object? value)
            where TContext    : IBuilderContext
            where TDescriptor : IImportDescriptor<TMember>
        {
            switch (value)
            {
                case IImportDescriptionProvider<TMember> provider:
                    descriptor.None();
                    provider.DescribeImport(ref descriptor);
                    break;

                case IImportDescriptionProvider provider:
                    descriptor.None();
                    provider.DescribeImport(ref descriptor);
                    break;

                case IResolve iResolve:
                    descriptor.Pipeline = (ResolveDelegate<TContext>)iResolve.Resolve;
                    return;

                case ResolveDelegate<TContext> resolver:
                    descriptor.Pipeline = resolver;
                    return;

                case IResolverFactory<Type> typeFactory:
                    descriptor.Pipeline = typeFactory.GetResolver<TContext>(descriptor.MemberType);
                    return;

                case Type target when typeof(Type) != descriptor.MemberType:
                    descriptor.Contract = new Contract(target);
                    descriptor.AllowDefault = false;
                    descriptor.None();
                    return;

                case UnityContainer.InvalidValue _:
                    descriptor.None();
                    return;

                default:
                    descriptor.Value = value;
                    return;
            }
        }

    }
}
