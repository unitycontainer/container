using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null != context.Target, "Target should never be null");

            var injection  = GetInjected<InjectionMemberInfo<TMemberInfo>>(context.Registration);
            var injections = injection;
            var members = GetMembers(context.Type);

            ReflectionInfo<TDependency> reflection = default;
            ImportInfo<TDependency> import = new ImportInfo<TDependency>(MemberType);

            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                import.Member = Unsafe.As<TDependency>(members[i]);

                var attribute = GetImportInfo(ref import);

                while(null != injection)
                {
                    if (MatchRank.ExactMatch == injection.Match(Unsafe.As<TMemberInfo>(import.Member)))
                    {
                        if (ImportType.Unknown == injection.GetImportInfo(ref import))
                            ParseImportData(ref import);

                        goto inject;
                    }

                    injection = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injection.Next);
                }

                if (ImportType.Attribute != attribute) goto MoveNext;
                    
                inject: var @override = context.GetOverride(in reflection.Import);

                if (null != @override)
                {
                    reflection.Data.Value = @override.Value;
                    do
                    {
                        reflection.Data = ParseImportData(ref reflection.Import, reflection.Data.Value);
                    }
                    while (ImportType.Unknown == reflection.Data.ImportType);
                }
                
                Build(ref context, in reflection.Import, in reflection.Data);

                // Rewind for the next member
                MoveNext: injection = injections;
            }
        }
    }
}
