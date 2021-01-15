using System;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override object? Analyse<TContext>(ref TContext context)
        {
            var type = context.Type;
            var members = GetDeclaredMembers(type);

            if (0 == members.Length) return null;

            var count = 0;
            var imports = new MemberDescriptor<TMemberInfo>[members.Length];

            // Load descriptor from metadata
            for (var i = 0; i < members.Length; i++)
            {
                ref var import = ref imports[i];

                import.MemberInfo = members[i];

                DescribeImport(ref import);
                if (import.IsImport) count++;
            }

            var injected = context.Injected<TMemberInfo, TData>();
            if (injected is null) return imports;

            // Add injection data
            int index;

            for (var member = injected;
                     member is not null;
                     member = (InjectionMember<TMemberInfo, TData>?)member.Next)
            {

                // TODO: Validation
                if (-1 == (index = IndexFromInjected(member, members)))
                    continue;

                ref var descriptor = ref imports[index];

                if (!descriptor.IsImport)
                {
                    count++;
                    descriptor.IsImport = true;
                }

                InjectImport<TContext>(ref descriptor, member);
            }

            if (0 == count) return null;

            var analytics = new ImportInfo[count];

            for (var i = count = 0; i < imports.Length; i++)
            {
                ref var import = ref imports[i];
                if (!import.IsImport) continue;
                
                //while(ImportType.Dynamic == import.ValueData.Type)

                analytics[count++] = new ImportInfo
                { 
                    Member   = import.MemberInfo,
                    Contract = new Contract(import.ContractType, import.ContractName),

                    Value   = import.ValueData,
                    Default = import.DefaultData
                };
            }

            return analytics;
        }
    }
}
