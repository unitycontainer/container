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
            ReflectionInfo<TDependency> info = default;

            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                info.Import.Member = Unsafe.As<TDependency>(members[i]);
                info.Data.DataType = ImportType.None;

                var attribute = GetImportInfo(ref info);

                while(null != injection)
                {
                    if (MatchRank.ExactMatch == injection.Match(Unsafe.As<TMemberInfo>(info.Import.Member)))
                    {
                        info.Data = ((IReflectionProvider<TDependency>)injection).GetReflectionInfo(ref info.Import);
                        while (ImportType.Unknown == info.Data.DataType)
                        { 
                            info.Data = ParseData(ref info.Import, info.Data.Value);
                        } 

                        goto inject;
                    }

                    injection = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injection.Next);
                }

                if (!attribute) goto MoveNext;
                    
                inject: var @override = context.GetOverride(in info.Import);

                if (null != @override)
                {
                    info.Data.Value = @override.Value;
                    do
                    {
                        info.Data = ParseData(ref info.Import, info.Data.Value);
                    }
                    while (ImportType.Unknown == info.Data.DataType);
                }
                
                Build(ref context, in info.Import, in info.Data);

                // Rewind for the next member
                MoveNext: injection = injections;
            }
        }
    }
}
