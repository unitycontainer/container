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

            var injected   = GetInjected<InjectionMemberInfo<TMemberInfo>>(context.Registration);
            var injections = injected;
            var members = GetMembers(context.Type);
            ImportInfo<TDependency> import = default;
            ImportData data = default;

            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                import.Member = Unsafe.As<TDependency>(members[i]);
                data.DataType = ImportType.None;

                var attribute = GetImportInfo(ref import);

                while(null != injected)
                {
                    if (MatchRank.ExactMatch == injected.Match(Unsafe.As<TMemberInfo>(import.Member)))
                        break;
                    
                    injected = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injected.Next);
                }

                if (attribute || null != injected)
                {
                    // Check for override
                    var @override = context.GetOverride(in import);

                    if (null != @override)
                    {
                        Build(ref context, in import, ParseData(ref import, @override.Value));
                    }
                    else if (null != injected)
                    {
                        data = ((IReflectionProvider<TDependency>)injected).GetReflectionInfo(ref import);
                        if (ImportType.Unknown == data.DataType) data = ParseData(ref import, data.Value);

                        Build(ref context, in import, in data);
                    }
                    else
                        Build(ref context, in import, in data);
                };

                // Rewind for the next member
                injected = injections;
            }
        }
    }
}
