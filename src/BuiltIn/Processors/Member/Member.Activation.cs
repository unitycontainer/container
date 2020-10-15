using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null != context.Target, "Target should never be null");

            ResolverOverride? @override;
            var injected   = GetInjected<InjectionMemberInfo<TMemberInfo>>(context.Registration);
            var injections = injected;

            foreach(var member in GetMembers(context.Type))
            {
                if (context.IsFaulted) return;

                // Injection first
                while(null != injected)
                {
                    if (MatchRank.ExactMatch == injected.Match(member))
                    {
                        var info = injected.GetInfo(member);

                        // Check for override
                        if (null != (@override = context.GetOverride(in info.Import)))
                        {
                            if (@override.Value is IReflectionProvider<TMemberInfo> provider)
                            {
                                var providerInfo = provider.GetInfo(member);
                                Build(ref context, in providerInfo.Import, in providerInfo.Data);
                            }
                            else
                                Build(ref context, in info.Import, AsImportData(Unsafe.As<TDependency>(member), @override.Value));
                        }
                        else
                            Build(ref context, in info.Import, in info.Data);

                        goto ContinueToNext;
                    }
                    
                    injected = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injected.Next);
                }
                
                // Annotation second
                var attribute = GetImportAttribute(Unsafe.As<TDependency>(member));
                if (null == attribute) goto ContinueToNext;

                var annotation = new ReflectionInfo<TMemberInfo>(member, attribute.ContractType ?? MemberType(Unsafe.As<TDependency>(member)), 
                                                                        attribute.ContractName, 
                                                                        attribute.AllowDefault);
                // Check for override
                if (null != (@override = context.GetOverride(in annotation.Import)))
                {
                    if (@override.Value is IReflectionProvider<TMemberInfo> provider)
                    {
                        var providerInfo = provider.GetInfo(member);
                        Build(ref context, in providerInfo.Import, in providerInfo.Data);
                    }
                    else
                        Build(ref context, in annotation.Import, AsImportData(Unsafe.As<TDependency>(member), @override.Value));
                }
                else
                    Build(ref context, in annotation.Import, in annotation.Data);

                // Rewind for the next member
                ContinueToNext: injected = injections;
            }
        }
    }
}
